using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.DTOs;
using Application.DTOs.Auth;
using Application.Features.Auth.GetAllUsers;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;
using System.Threading;
using Xunit;

namespace Tests.Handlers.Users
{
    public class GetAllUsersHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IMemoryCacheService> _cacheMock;
        private readonly GetAllUsersHandler _handler;

        public GetAllUsersHandlerTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _cacheMock = new Mock<IMemoryCacheService>();
            _handler = new GetAllUsersHandler(_userRepoMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_CacheHit_ReturnsCachedResult()
        {
            // Arrange
            var query = new GetAllUsersQuery { PageNumber = 1, PageSize = 2 };
            var cachedResult = new PaginatedResult<UserDto>
            {
                Items = new List<UserDto>
            {
                new UserDto { Id = Guid.NewGuid(), Email = "cached@example.com", FullName = "Cached User", CreatedAt = DateTime.UtcNow }
            },
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 2
            };

            _cacheMock.Setup(c => c.GetAsync<PaginatedResult<UserDto>>("users:1:2"))
                      .ReturnsAsync(cachedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(1, result.Items.Count);
            Assert.Equal("cached@example.com", result.Items[0].Email);
            _userRepoMock.Verify(r => r.GetAllPaginatedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CacheMiss_FetchesFromRepoAndCaches()
        {
            // Arrange
            var query = new GetAllUsersQuery { PageNumber = 1, PageSize = 2 };

            _cacheMock.Setup(c => c.GetAsync<PaginatedResult<UserDto>>("users:1:2"))
                      .ReturnsAsync((PaginatedResult<UserDto>)null);

            var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), Email = Email.Create("test1@example.com"), FullName = "Test One", CreatedAt = DateTime.UtcNow },
            new User { Id = Guid.NewGuid(), Email = Email.Create("test2@example.com"), FullName = "Test Two", CreatedAt = DateTime.UtcNow }
        };

            _userRepoMock.Setup(r => r.GetAllPaginatedAsync(1, 2))
                         .ReturnsAsync((users, 10));

            _cacheMock.Setup(c => c.SetAsync(
                "users:1:2",
                It.IsAny<PaginatedResult<UserDto>>(),
                It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(10, result.TotalCount);
            _userRepoMock.Verify(r => r.GetAllPaginatedAsync(1, 2), Times.Once);
            _cacheMock.Verify(c => c.SetAsync("users:1:2", It.IsAny<PaginatedResult<UserDto>>(), TimeSpan.FromMinutes(3)), Times.Once);
        }

        [Fact]
        public async Task Handle_NoUsers_ReturnsEmptyResult()
        {
            // Arrange
            var query = new GetAllUsersQuery { PageNumber = 1, PageSize = 5 };

            _cacheMock.Setup(c => c.GetAsync<PaginatedResult<UserDto>>("users:1:5"))
                      .ReturnsAsync((PaginatedResult<UserDto>)null);

            _userRepoMock.Setup(r => r.GetAllPaginatedAsync(1, 5))
                         .ReturnsAsync((new List<User>(), 0));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }
    }

}
