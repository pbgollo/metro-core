using System.Net;
using Metro.Domain.Users.Authentication;
using Metro.Domain.Users.Authentication.Services;
using Metro.Domain.Users.Commands;
using Metro.Domain.Users.Entities;
using Metro.Domain.Users.Handlers;
using Metro.Domain.Users.Repositories;
using Metro.Domain.Users.ViewModel;
using Metro.Shared.Data;
using Metro.Shared.Results;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;

namespace Metro.Domain.Tests.Users.Handlers;

public class CreateUserHandlerTests
{
    private readonly IUnityOfWork _unityOfWork = Substitute.For<IUnityOfWork>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IUserQueryRepository _userQueryRepository = Substitute.For<IUserQueryRepository>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly CreateUserHandler _sut;

    public CreateUserHandlerTests()
    {
        _passwordService.HashPassword(Arg.Any<string>()).Returns("hashed-password");
        _sut = new CreateUserHandler(_unityOfWork, _userRepository, _userQueryRepository, _passwordService);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ReturnsConflict_AndDoesNotPersist()
    {
        var command = CreateCommand();
        _userQueryRepository.GetByEmail(command.Email).Returns(new GetUserViewModel
        {
            Id = Guid.NewGuid(),
            Name = "Existing",
            Email = command.Email,
            Document = "1",
            Phone = "1",
            Role = "client",
            IsActive = true
        });

        var result = await _sut.Handle(command, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        result.Message.ShouldBe("O e-mail já está cadastrado.");
        result.Data.ShouldBeNull();
        await _userRepository.DidNotReceive().Create(Arg.Any<User>());
        await _unityOfWork.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
        await _unityOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenValid_CreatesUser_AndReturnsCreatedWithId()
    {
        var command = CreateCommand(role: "master");
        _userQueryRepository.GetByEmail(command.Email).Returns((GetUserViewModel?)null);

        User? created = null;
        _userRepository
            .When(x => x.Create(Arg.Any<User>()))
            .Do(ci => created = ci.Arg<User>());

        var result = await _sut.Handle(command, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.Data.ShouldNotBeNull();
        result.Data!.Id.ShouldBe(created!.Id);
        created.Name.ShouldBe(command.Name);
        created.Email.ShouldBe(command.Email);
        created.Role.ShouldBe("master");
        created.IsActive.ShouldBeTrue();

        await _unityOfWork.Received(1).BeginAsync(Arg.Any<CancellationToken>());
        await _userRepository.Received(1).Create(Arg.Any<User>());
        await _unityOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        await _unityOfWork.DidNotReceive().RollbackAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("admin")]
    [InlineData("")]
    [InlineData("CLIENT")]
    public async Task Handle_WhenRoleInvalid_DefaultsToClient(string role)
    {
        var command = CreateCommand(role: role);
        _userQueryRepository.GetByEmail(command.Email).Returns((GetUserViewModel?)null);

        User? created = null;
        _userRepository
            .When(x => x.Create(Arg.Any<User>()))
            .Do(ci => created = ci.Arg<User>());

        await _sut.Handle(command, CancellationToken.None);

        created!.Role.ShouldBe("client");
    }

    [Fact]
    public async Task Handle_WhenPasswordInvalid_ReturnsBadRequest()
    {
        var command = CreateCommand();
        command.Password = "123";
        _userQueryRepository.GetByEmail(command.Email).Returns((GetUserViewModel?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Message.ShouldBe(PasswordPolicy.RequirementsMessage);
        await _userRepository.DidNotReceive().Create(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_WhenPersistFails_RollsBack_AndRethrows()
    {
        var command = CreateCommand();
        _userQueryRepository.GetByEmail(command.Email).Returns((GetUserViewModel?)null);
        _userRepository
            .Create(Arg.Any<User>())
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Should.ThrowAsync<InvalidOperationException>(() => _sut.Handle(command, CancellationToken.None));
        await _unityOfWork.Received(1).BeginAsync(Arg.Any<CancellationToken>());
        await _unityOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _unityOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    private static CreateUserCommand CreateCommand(string role = "client") => new()
    {
        Name = "Ana Silva",
        Email = "ana@example.com",
        Document = "12345678900",
        Phone = "11999999999",
        Password = "Secret123!",
        Role = role
    };
}
