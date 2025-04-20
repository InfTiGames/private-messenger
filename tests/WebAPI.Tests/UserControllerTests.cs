using Moq;
using NUnit.Framework;
using Application.Interfaces;
using WebAPI.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

[TestFixture]
public class UserControllerTests
{
    private Mock<IUserRepository> _userRepoMock;
    private Mock<MessageCacheService> _cacheServiceMock;
    private Mock<ILogger<UserController>> _loggerMock;
    private UserController _controller;

    [SetUp]
    public void Setup()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _cacheServiceMock = new Mock<MessageCacheService>();
        _loggerMock = new Mock<ILogger<UserController>>();
        _controller = new UserController(_userRepoMock.Object, null, _loggerMock.Object, null);
    }

    [Test]
    public async Task DeleteUserData_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        _userRepoMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        // Act
        var result = await _controller.DeleteUserData();

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }
}
