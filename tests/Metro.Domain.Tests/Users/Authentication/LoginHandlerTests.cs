using System.Net;
using FluentAssertions;
using Metro.Domain.Users.Authentication.Handlers;
using Metro.Domain.Users.Authentication.Queries;
using Metro.Domain.Users.Authentication.Services;
using Metro.Domain.Users.Entities;
using Metro.Domain.Users.Repositories;
using NSubstitute;

namespace Metro.Domain.Tests.Users.Authentication;

public class LoginHandlerTests
{
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IPasswordService _passwordService = Substitute.For<IPasswordService>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly LoginHandler _sut;

    public LoginHandlerTests()
    {
        _sut = new LoginHandler(_tokenService, _passwordService, _userRepository);
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

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        _tokenService.DidNotReceive().GenerateToken(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_WhenPasswordInvalid_ReturnsUnauthorized()
    {
        var user = CreateActiveUser();
        _userRepository.GetEmail(user.Email).Returns(user);
        _passwordService.ConfirmPassword(Arg.Any<byte[]>(), "wrong").Returns(false);

        var result = await _sut.Handle(new LoginQuery
        {
            Email = user.Email,
            Password = "wrong"
        }, CancellationToken.None);

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        _tokenService.DidNotReceive().GenerateToken(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_WhenUserInactive_ReturnsUnauthorized()
    {
        var user = CreateActiveUser(isActive: false);
        _userRepository.GetEmail(user.Email).Returns(user);
        _passwordService.ConfirmPassword(Arg.Any<byte[]>(), "Secret123!").Returns(true);

        var result = await _sut.Handle(new LoginQuery
        {
            Email = user.Email,
            Password = "Secret123!"
        }, CancellationToken.None);

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        _tokenService.DidNotReceive().GenerateToken(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_WhenCredentialsValid_ReturnsOkWithToken()
    {
        var user = CreateActiveUser();
        _userRepository.GetEmail(user.Email).Returns(user);
        _passwordService.ConfirmPassword(Arg.Any<byte[]>(), "Secret123!").Returns(true);
        _tokenService.GenerateToken(user).Returns("jwt-token");

        var result = await _sut.Handle(new LoginQuery
        {
            Email = user.Email,
            Password = "Secret123!"
        }, CancellationToken.None);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Data.Token.Should().Be("jwt-token");
        _tokenService.Received(1).GenerateToken(user);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsNotBase64_ReturnsUnauthorized()
    {
        var user = new User("Ana", "ana@example.com", "1", "1", "not-base64!!!", "client", isActive: true);
        _userRepository.GetEmail(user.Email).Returns(user);

        var result = await _sut.Handle(new LoginQuery
        {
            Email = user.Email,
            Password = "Secret123!"
        }, CancellationToken.None);

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        _passwordService.DidNotReceive().ConfirmPassword(Arg.Any<byte[]>(), Arg.Any<string>());
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
