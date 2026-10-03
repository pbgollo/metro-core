using Metro.Application.Results;
using Metro.Domain.Services;
using Metro.Application.Auth.Commands;
using Metro.Application.Auth.Handlers;
using Metro.Domain.Auth.Services;
using Metro.Domain.Users.Entities;
using Metro.Domain.Users.Repositories;
using Metro.Shared.Data;
using NSubstitute;
using Shouldly;

namespace Metro.Application.Tests.Auth;

public class RequestPasswordRecoveryHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordRecoveryCodeRepository _passwordRecoveryCodeRepository = Substitute.For<IPasswordRecoveryCodeRepository>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly IPasswordRecoverySettings _settings = Substitute.For<IPasswordRecoverySettings>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IUnityOfWork _unityOfWork = Substitute.For<IUnityOfWork>();
    private readonly RequestPasswordRecoveryHandler _sut;

    public RequestPasswordRecoveryHandlerTests()
    {
        _settings.CodeLength.Returns(6);
        _settings.CodeExpiresInMinutes.Returns(15);
        _settings.MaxAttempts.Returns(5);
        _sut = new RequestPasswordRecoveryHandler(
            _userRepository,
            _passwordRecoveryCodeRepository,
            _passwordService,
            _settings,
            _emailService,
            _unityOfWork);
    }

    [Fact]
    public async Task Handle_WhenEmailMissing_ReturnsOkWithoutSendingEmail()
    {
        var result = await _sut.Handle(new RequestPasswordRecoveryCommand
        {
            Email = " "
        }, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        await _emailService.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        await _unityOfWork.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsOkWithoutSendingEmail()
    {
        _userRepository.GetEmail("ana@example.com").Returns((User?)null);

        var result = await _sut.Handle(new RequestPasswordRecoveryCommand
        {
            Email = "ana@example.com"
        }, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        await _emailService.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenUserExists_CreatesCodeAndSendsEmail()
    {
        var user = CreateActiveUser();
        _userRepository.GetEmail(user.Email).Returns(user);
        _passwordService.GenerateNumericCode(6).Returns("123456");
        _passwordService.HashCode("123456").Returns("hash");

        var result = await _sut.Handle(new RequestPasswordRecoveryCommand
        {
            Email = user.Email
        }, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        await _passwordRecoveryCodeRepository.Received(1).InvalidateAllActiveByUserId(user.Id);
        await _passwordRecoveryCodeRepository.Received(1).Create(Arg.Is<PasswordRecoveryCode>(c =>
            c.UserId == user.Id && c.CodeHash == "hash"));
        await _emailService.Received(1).Send(
            user.Email,
            "Recuperação de senha",
            Arg.Is<string>(html => html.Contains("123456")));
        await _unityOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    private static User CreateActiveUser() => new(
        name: "Ana",
        email: "ana@example.com",
        document: "1",
        phone: "1",
        password: Convert.ToBase64String(new byte[] { 1, 2, 3, 4 }),
        role: "client",
        isActive: true);
}
