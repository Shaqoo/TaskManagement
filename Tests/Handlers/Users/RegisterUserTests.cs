using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Handlers.Users
{
    using Application.Abstractions;
    using Application.DTOs.Auth;
    using Application.Features.Auth.Register;
    using Domain.Entities;
    using Domain.Enums;
    using Domain.ValueObjects;
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class RegisterHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly RegisterHandler _handler;

        public RegisterHandlerTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();

            _handler = new RegisterHandler(
                _userRepoMock.Object,
                _jwtServiceMock.Object,
                _unitOfWorkMock.Object,
                _passwordHasherMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsAuthResponse()
        {
            // Arrange
            var command = new RegisterCommand
            {
                FullName = "John Doe",
                Email = "john@example.com",
                Password = "SecurePassword123"
            };

            var hashedPassword = "hashedpassword";
            var fakeUserId = Guid.NewGuid();

            _passwordHasherMock
                .Setup(p => p.HashPassword(It.IsAny<User>(), command.Password))
                .Returns(hashedPassword);

            _userRepoMock
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => u.Id = fakeUserId)
                .Returns(Task.CompletedTask);

            //_unitOfWorkMock
            //    .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            //    .Returns(Task});

            _jwtServiceMock
                .Setup(j => j.GenerateToken(It.IsAny<Guid>(), command.Email, Role.User.ToString()))
                .Returns("jwt-token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.Email, result.Email);
            Assert.Equal("jwt-token", result.Token);
            Assert.Equal(Role.User.ToString(), result.Role);
        }

        [Fact]
        public async Task Handle_AddUserFails_ThrowsException()
        {
            // Arrange
            var command = new RegisterCommand
            {
                FullName = "John Doe",
                Email = Email.Create("john@example.com"),
                Password = "Password"
            };

            _passwordHasherMock
                .Setup(p => p.HashPassword(It.IsAny<User>(), command.Password))
                .Returns("hashed");

            _userRepoMock
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .ThrowsAsync(new Exception("DB failure"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }

}
