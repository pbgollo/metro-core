using System.Net;
using Metro.Domain.Users.Commands;
using Metro.Domain.Users.Entities;
using Metro.Domain.Users.Handlers;
using Metro.Domain.Users.Repositories;
using Metro.Shared.Data;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;

namespace Metro.Domain.Tests.Users.Handlers;

public class DeleteUserHandlerTests
{
    private readonly IUnityOfWork _unityOfWork = Substitute.For<IUnityOfWork>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly DeleteUserHandler _sut;

    public DeleteUserHandlerTests()
    {
        _sut = new DeleteUserHandler(_unityOfWork, _userRepository);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _userRepository.GetById(id).Returns((User?)null);

        var result = await _sut.Handle(new DeleteUserCommand { Id = id }, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await _userRepository.DidNotReceive().Delete(Arg.Any<User>());
        await _unityOfWork.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenValid_DeletesUser_AndReturnsOk()
    {
        var user = new User("Ana", "ana@example.com", "1", "1", "pwd", "client");
        _userRepository.GetById(user.Id).Returns(user);

        var result = await _sut.Handle(new DeleteUserCommand { Id = user.Id }, CancellationToken.None);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        await _unityOfWork.Received(1).BeginAsync(Arg.Any<CancellationToken>());
        await _userRepository.Received(1).Delete(user);
        await _unityOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPersistFails_RollsBack_AndRethrows()
    {
        var user = new User("Ana", "ana@example.com", "1", "1", "pwd", "client");
        _userRepository.GetById(user.Id).Returns(user);
        _userRepository.Delete(user).ThrowsAsync(new InvalidOperationException("db error"));

        await Should.ThrowAsync<InvalidOperationException>(() =>
            _sut.Handle(new DeleteUserCommand { Id = user.Id }, CancellationToken.None));
        await _unityOfWork.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _unityOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
