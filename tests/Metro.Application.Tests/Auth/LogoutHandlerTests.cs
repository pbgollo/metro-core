using Metro.Application.Results;
using Metro.Application.Auth.Commands;
using Metro.Application.Auth.Handlers;
using Metro.Domain.Auth.Services;
using Metro.Shared.Data;
using NSubstitute;
using Shouldly;
using Metro.Domain.Auth.Entities;
using Metro.Domain.Auth.Repositories;

namespace Metro.Application.Tests.Auth;

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

        result.Status.ShouldBe(ResultStatus.Ok);
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

        result.Status.ShouldBe(ResultStatus.Ok);
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

        result.Status.ShouldBe(ResultStatus.Ok);
        existing.IsActive.ShouldBeFalse();
        await _refreshTokenRepository.Received(1).Update(existing);
        await _unityOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
