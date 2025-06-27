using Application.Abstractions;
using Application.Features.Auth.Login;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Moq;


namespace Tests.Handlers.Users
{
    public class LoginHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly LoginHandler _handler;

        public LoginHandlerTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();

            _handler = new LoginHandler(_userRepoMock.Object, _jwtServiceMock.Object, _passwordHasherMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = Email.Create("test@example.com"),
                PasswordHash = "hashedpassword",
                Role = Domain.Enums.Role.Admin
            };

            var request = new LoginQuery { Email = user.Email, Password = "plaintextPassword" };

            _userRepoMock.Setup(x => x.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, request.Password))
                               .Returns(PasswordVerificationResult.Success);
            _jwtServiceMock.Setup(x => x.GenerateToken(user.Id, user.Email, user.Role.ToString()))
                           .Returns("jwt-token");

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal("jwt-token", result.Token);
            Assert.Equal("Admin", result.Role);
        }

        [Fact]
        public async Task Handle_InvalidEmail_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new LoginQuery { Email = "notfound@example.com", Password = "password" };

            _userRepoMock.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_InvalidPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = Email.Create("test@example.com"),
                PasswordHash = "hashedpassword",
                Role = Domain.Enums.Role.Admin
            };

            var request = new LoginQuery { Email = user.Email, Password = "wrongPassword" };

            _userRepoMock.Setup(x => x.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, request.Password))
                               .Returns(PasswordVerificationResult.Failed);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }
    }

}
