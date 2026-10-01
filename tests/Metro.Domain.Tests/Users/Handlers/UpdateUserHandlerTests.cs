using System.Net;
using FluentAssertions;
using Metro.Domain.Services;
using Metro.Domain.Users.Authentication.Services;
using Metro.Domain.Users.Commands;
using Metro.Domain.Users.Entities;
using Metro.Domain.Users.Handlers;
using Metro.Domain.Users.Repositories;
using Metro.Shared.Data;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Metro.Domain.Tests.Users.Handlers;

public class UpdateUserHandlerTests
{
    private readonly IUnityOfWork _unityOfWork = Substitute.For<IUnityOfWork>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly UpdateUserHandler _sut;

    public UpdateUserHandlerTests()
    {
        _passwordService.HashPasswordWithSalt(Arg.Any<string>()).Returns(new byte[] { 9, 8, 7, 6 });
        _sut = new UpdateUserHandler(_unityOfWork, _userRepository, _emailService, _passwordService);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFound()
    {
        var command = CreateCommand(Guid.NewGuid());
        _userRepository.GetById(command.Id).Returns((User?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _unityOfWork.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceive().Update(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_WhenValid_UpdatesUser_AndReturnsOk()
    {
        var user = CreateUser();
        var command = CreateCommand(user.Id, role: "master", isActive: true);
        _userRepository.GetById(user.Id).Returns(user);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        user.Name.Should().Be(command.Name);
        user.Email.Should().Be(command.Email);
        user.Role.Should().Be("master");
        user.IsActive.Should().BeTrue();

        await _unityOfWork.Received(1).BeginAsync(Arg.Any<CancellationToken>());
        await _userRepository.Received(1).Update(user);
        await _unityOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPasswordProvided_UpdatesPasswordHash()
    {
        var user = CreateUser();
        var originalPassword = user.Password;
        var command = CreateCommand(user.Id, password: "NewPassword!");
        _userRepository.GetById(user.Id).Returns(user);

        await _sut.Handle(command, CancellationToken.None);

        _passwordService.Received(1).HashPasswordWithSalt("NewPassword!");
        user.Password.Should().NotBe(originalPassword);
        user.Password.Should().Be(Convert.ToBase64String(new byte[] { 9, 8, 7, 6 }));
    }

    [Fact]
    public async Task Handle_WhenPasswordEmpty_DoesNotHashPassword()
    {
        var user = CreateUser();
        var command = CreateCommand(user.Id, password: "   ");
        _userRepository.GetById(user.Id).Returns(user);

        await _sut.Handle(command, CancellationToken.None);

        _passwordService.DidNotReceive().HashPasswordWithSalt(Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenPersistFails_RollsBack_AndRethrows()
    {
        var user = CreateUser();
        var command = CreateCommand(user.Id);
        _userRepository.GetById(user.Id).Returns(user);
        _userRepository.Update(user).ThrowsAsync(new InvalidOperationException("db error"));

        var act = () => _sut.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        await _unityOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _unityOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    private static User CreateUser() => new(
        name: "Old Name",
        email: "old@example.com",
        document: "111",
        phone: "111",
        password: Convert.ToBase64String(new byte[] { 1, 2, 3 }),
        role: "client",
        isActive: false);

    private static UpdateUserCommand CreateCommand(
        Guid id,
        string? password = null,
        string role = "client",
        bool? isActive = null) => new()
    {
        Id = id,
        Name = "New Name",
        Email = "new@example.com",
        Document = "222",
        Phone = "222",
        Password = password,
        Role = role,
        IsActive = isActive
    };
}
