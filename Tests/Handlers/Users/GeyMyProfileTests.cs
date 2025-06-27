using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Handlers.Users
{
    using Application.Abstractions;
    using Application.DTOs;
    using Application.DTOs.Auth;
    using Application.Features.Auth.GetUser;
    using Domain.Entities;
    using Domain.ValueObjects;
    using Moq;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class GetMyProfileHandlerTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly GetMyProfileHandler _handler;

        public GetMyProfileHandlerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _userRepoMock = new Mock<IUserRepository>();

            _handler = new GetMyProfileHandler(
                _currentUserServiceMock.Object,
                _userRepoMock.Object
            );
        }

        [Fact]
        public async Task Handle_UserAuthenticatedAndExists_ReturnsUserDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

            var user = new User
            {
                Id = userId,
                Email = Email.Create("test@example.com"),
                FullName = "Test User",
                CreatedAt = DateTime.UtcNow
            };

            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(new GetMyProfileQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(user.Email, result.Data.Email);
            Assert.Equal(user.FullName, result.Data.FullName);
        }

        [Fact]
        public async Task Handle_UserIsNull_ReturnsFailure()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
            _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(new GetMyProfileQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("User not found.", result.Message);
        }

        [Fact]
        public async Task Handle_CurrentUserServiceIsNull_ReturnsFailure()
        {
            // Arrange: Simulate null current user service by passing null to the constructor
            var handlerWithNullService = new GetMyProfileHandler(null, _userRepoMock.Object);

            // Act
            var result = await handlerWithNullService.Handle(new GetMyProfileQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("User not authenticated.", result.Message);
        }
    }

}
