using Metro.Application.Results;
using Metro.Application.Auth.Commands;
using Metro.Application.Auth.Handlers;
using Metro.Domain.Auth.Services;
using Metro.Domain.Users.Entities;
using Metro.Domain.Users.Repositories;
using Metro.Shared.Data;
using NSubstitute;
using Shouldly;

namespace Metro.Application.Tests.Auth;

public class ResetPasswordHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordRecoveryCodeRepository _passwordRecoveryCodeRepository = Substitute.For<IPasswordRecoveryCodeRepository>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly IUnityOfWork _unityOfWork = Substitute.For<IUnityOfWork>();
    private readonly ResetPasswordHandler _sut;

    public ResetPasswordHandlerTests()
    {
        _sut = new ResetPasswordHandler(
            _userRepository,
            _passwordRecoveryCodeRepository,
            _refreshTokenRepository,
            _passwordService,
            _unityOfWork);
    }

    [Fact]
    public async Task Handle_WhenCodeValid_UpdatesPasswordAndRevokesRefreshTokens()
    {
        var user = CreateActiveUser();
        var recovery = new PasswordRecoveryCode(user.Id, "hash", DateTime.UtcNow.AddMinutes(15), 5);
        _userRepository.GetEmail(user.Email).Returns(user);
        _passwordRecoveryCodeRepository.GetActiveByUserId(user.Id).Returns(recovery);
        _passwordService.HashCode("123456").Returns("hash");
        _passwordService.HashPassword("NovaSenha1").Returns("hashed-password");

        var result = await _sut.Handle(new ResetPasswordCommand
        {
            Email = user.Email,
            Code = "123456",
            NewPassword = "NovaSenha1"
        }, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        recovery.UsedAt.ShouldNotBeNull();
        await _userRepository.Received(1).Update(user);
        await _refreshTokenRepository.Received(1).RevokeAllActiveByUserId(user.Id);
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
