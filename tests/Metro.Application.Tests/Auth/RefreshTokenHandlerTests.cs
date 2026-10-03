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

public class RefreshTokenHandlerTests
{
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IUnityOfWork _unityOfWork = Substitute.For<IUnityOfWork>();
    private readonly RefreshTokenHandler _sut;

    public RefreshTokenHandlerTests()
    {
        _sut = new RefreshTokenHandler(
            _tokenService,
            _userRepository,
            _refreshTokenRepository,
            _unityOfWork);
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenNotFound_ReturnsUnauthorized()
    {
        _tokenService.HashRefreshToken("old-refresh").Returns("hash");
        _refreshTokenRepository.GetByTokenHash("hash").Returns((RefreshToken?)null);

        var result = await _sut.Handle(new RefreshTokenCommand
        {
            RefreshToken = "old-refresh"
        }, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
        await _unityOfWork.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenRevoked_ReturnsUnauthorized()
    {
        var existing = new RefreshToken(Guid.NewGuid(), "hash", DateTime.UtcNow.AddDays(1));
        existing.Revoke();
        _tokenService.HashRefreshToken("old-refresh").Returns("hash");
        _refreshTokenRepository.GetByTokenHash("hash").Returns(existing);

        var result = await _sut.Handle(new RefreshTokenCommand
        {
            RefreshToken = "old-refresh"
        }, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
        await _unityOfWork.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserInactive_ReturnsUnauthorized()
    {
        var user = CreateActiveUser(isActive: false);
        var existing = new RefreshToken(user.Id, "hash", DateTime.UtcNow.AddDays(1));
        _tokenService.HashRefreshToken("old-refresh").Returns("hash");
        _refreshTokenRepository.GetByTokenHash("hash").Returns(existing);
        _userRepository.GetById(user.Id).Returns(user);

        var result = await _sut.Handle(new RefreshTokenCommand
        {
            RefreshToken = "old-refresh"
        }, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
        await _unityOfWork.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenValid_RotatesAndReturnsNewTokens()
    {
        var user = CreateActiveUser();
        var existing = new RefreshToken(user.Id, "hash", DateTime.UtcNow.AddDays(1));
        _tokenService.HashRefreshToken("old-refresh").Returns("hash");
        _tokenService.GenerateRefreshToken().Returns("new-refresh");
        _tokenService.HashRefreshToken("new-refresh").Returns("new-hash");
        _tokenService.GetRefreshTokenExpiresAt().Returns(DateTime.UtcNow.AddDays(7));
        _tokenService.GenerateAccessToken(user).Returns("access-token");
        _tokenService.GetAccessTokenExpiresInSeconds().Returns(1800);
        _refreshTokenRepository.GetByTokenHash("hash").Returns(existing);
        _userRepository.GetById(user.Id).Returns(user);

        var result = await _sut.Handle(new RefreshTokenCommand
        {
            RefreshToken = "old-refresh"
        }, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Ok);
        result.Data.ShouldNotBeNull();
        result.Data!.AccessToken.ShouldBe("access-token");
        result.Data.RefreshToken.ShouldBe("new-refresh");
        result.Data.ExpiresIn.ShouldBe(1800);
        existing.IsActive.ShouldBeFalse();
        await _unityOfWork.Received(1).BeginAsync(Arg.Any<CancellationToken>());
        await _refreshTokenRepository.Received(1).Update(existing);
        await _refreshTokenRepository.Received(1).Create(Arg.Is<RefreshToken>(t =>
            t.UserId == user.Id && t.TokenHash == "new-hash"));
        await _unityOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    private static User CreateActiveUser(bool isActive = true) => new(
        name: "Ana",
        email: "ana@example.com",
        document: "1",
        phone: "1",
        password: Convert.ToBase64String(new byte[] { 1, 2, 3, 4 }),
        role: "client",
        isActive: isActive);
}
