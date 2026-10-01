using System.Net;
using Metro.Domain.Users.Authentication.Handlers;
using Metro.Domain.Users.Authentication.Queries;
using Metro.Domain.Users.Authentication.Services;
using Metro.Domain.Users.Entities;
using Metro.Domain.Users.Repositories;
using Metro.Shared.Data;
using NSubstitute;
using Shouldly;

namespace Metro.Domain.Tests.Users.Authentication;

public class LoginHandlerTests
{
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IUnityOfWork _unityOfWork = Substitute.For<IUnityOfWork>();
    private readonly LoginHandler _sut;

    public LoginHandlerTests()
    {
        _sut = new LoginHandler(
            _tokenService,
            _passwordService,
            _userRepository,
            _refreshTokenRepository,
            _unityOfWork);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsUnauthorized()
    {
        _userRepository.GetEmail("ana@example.com").Returns((User?)null);

        var result = await _sut.Handle(new LoginQuery
        {
            Email = "ana@example.com",
            Password = "Secret123!"
        }, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        result.Data.ShouldBeNull();
        _tokenService.DidNotReceive().GenerateAccessToken(Arg.Any<User>());
        await _unityOfWork.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPasswordInvalid_ReturnsUnauthorized()
    {
        var user = CreateActiveUser();
        _userRepository.GetEmail(user.Email).Returns(user);
        _passwordService.ConfirmPassword(user.Password, "wrong").Returns(false);

        var result = await _sut.Handle(new LoginQuery
        {
            Email = user.Email,
            Password = "wrong"
        }, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        _tokenService.DidNotReceive().GenerateAccessToken(Arg.Any<User>());
        await _unityOfWork.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserInactive_ReturnsUnauthorized()
    {
        var user = CreateActiveUser(isActive: false);
        _userRepository.GetEmail(user.Email).Returns(user);
        _passwordService.ConfirmPassword(user.Password, "Secret123!").Returns(true);

        var result = await _sut.Handle(new LoginQuery
        {
            Email = user.Email,
            Password = "Secret123!"
        }, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        _tokenService.DidNotReceive().GenerateAccessToken(Arg.Any<User>());
        await _unityOfWork.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCredentialsValid_ReturnsOkWithAccessAndRefreshTokens()
    {
        var user = CreateActiveUser();
        _userRepository.GetEmail(user.Email).Returns(user);
        _passwordService.ConfirmPassword(user.Password, "Secret123!").Returns(true);
        _tokenService.GenerateRefreshToken().Returns("refresh-token");
        _tokenService.HashRefreshToken("refresh-token").Returns("refresh-hash");
        _tokenService.GetRefreshTokenExpiresAt().Returns(DateTime.UtcNow.AddDays(7));
        _tokenService.GenerateAccessToken(user).Returns("access-token");
        _tokenService.GetAccessTokenExpiresInSeconds().Returns(1800);

        var result = await _sut.Handle(new LoginQuery
        {
            Email = user.Email,
            Password = "Secret123!"
        }, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data!.AccessToken.ShouldBe("access-token");
        result.Data.RefreshToken.ShouldBe("refresh-token");
        result.Data.ExpiresIn.ShouldBe(1800);
        await _unityOfWork.Received(1).BeginAsync(Arg.Any<CancellationToken>());
        await _refreshTokenRepository.Received(1).RevokeAllActiveByUserId(user.Id);
        await _refreshTokenRepository.Received(1).Create(Arg.Is<RefreshToken>(t =>
            t.UserId == user.Id && t.TokenHash == "refresh-hash"));
        await _unityOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    private static User CreateActiveUser(bool isActive = true) => new(
        name: "Ana",
        email: "ana@example.com",
        document: "1",
        phone: "1",
        password: "AQAAAA-stored-hash",
        role: "client",
        isActive: isActive);
}
