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

public class LogoutHandlerTests
{
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IUnityOfWork _unityOfWork = Substitute.For<IUnityOfWork>();
    private readonly LogoutHandler _sut;

    public LogoutHandlerTests()
    {
        _sut = new LogoutHandler(_tokenService, _refreshTokenRepository, _unityOfWork);
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenMissing_ReturnsOkWithoutPersisting()
    {
        var result = await _sut.Handle(new LogoutCommand
        {
            RefreshToken = " "
        }, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        await _unityOfWork.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenNotFound_ReturnsOkWithoutPersisting()
    {
        _tokenService.HashRefreshToken("refresh").Returns("hash");
        _refreshTokenRepository.GetByTokenHash("hash").Returns((RefreshToken?)null);

        var result = await _sut.Handle(new LogoutCommand
        {
            RefreshToken = "refresh"
        }, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        await _unityOfWork.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenActive_RevokesAndReturnsOk()
    {
        var existing = new RefreshToken(Guid.NewGuid(), "hash", DateTime.UtcNow.AddDays(1));
        _tokenService.HashRefreshToken("refresh").Returns("hash");
        _refreshTokenRepository.GetByTokenHash("hash").Returns(existing);

        var result = await _sut.Handle(new LogoutCommand
        {
            RefreshToken = "refresh"
        }, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        existing.IsActive.ShouldBeFalse();
        await _refreshTokenRepository.Received(1).Update(existing);
        await _unityOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
