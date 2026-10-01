using System.Net;
using Metro.Domain.Users.Authentication.Commands;
using Metro.Domain.Users.Authentication.Handlers;
using Metro.Domain.Users.Authentication.Services;
using Metro.Domain.Users.Entities;
using Metro.Domain.Users.Repositories;
using Metro.Shared.Data;
using NSubstitute;
using Shouldly;

namespace Metro.Domain.Tests.Users.Authentication;

public class VerifyPasswordRecoveryCodeHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordRecoveryCodeRepository _passwordRecoveryCodeRepository = Substitute.For<IPasswordRecoveryCodeRepository>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly IUnityOfWork _unityOfWork = Substitute.For<IUnityOfWork>();
    private readonly VerifyPasswordRecoveryCodeHandler _sut;

    public VerifyPasswordRecoveryCodeHandlerTests()
    {
        _sut = new VerifyPasswordRecoveryCodeHandler(
            _userRepository,
            _passwordRecoveryCodeRepository,
            _passwordService,
            _unityOfWork);
    }

    [Fact]
    public async Task Handle_WhenCodeInvalid_IncrementsAttemptsAndReturnsUnauthorized()
    {
        var user = CreateActiveUser();
        var recovery = new PasswordRecoveryCode(user.Id, "hash", DateTime.UtcNow.AddMinutes(15), 5);
        _userRepository.GetEmail(user.Email).Returns(user);
        _passwordRecoveryCodeRepository.GetActiveByUserId(user.Id).Returns(recovery);
        _passwordService.HashCode("000000").Returns("wrong");

        var result = await _sut.Handle(new VerifyPasswordRecoveryCodeCommand
        {
            Email = user.Email,
            Code = "000000"
        }, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        recovery.AttemptCount.ShouldBe(1);
        await _passwordRecoveryCodeRepository.Received(1).Update(recovery);
        await _unityOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCodeValid_ReturnsOk()
    {
        var user = CreateActiveUser();
        var recovery = new PasswordRecoveryCode(user.Id, "hash", DateTime.UtcNow.AddMinutes(15), 5);
        _userRepository.GetEmail(user.Email).Returns(user);
        _passwordRecoveryCodeRepository.GetActiveByUserId(user.Id).Returns(recovery);
        _passwordService.HashCode("123456").Returns("hash");

        var result = await _sut.Handle(new VerifyPasswordRecoveryCodeCommand
        {
            Email = user.Email,
            Code = "123456"
        }, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        await _unityOfWork.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
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
