using Diquis.Domain.Entities.Common;
using Diquis.Infrastructure.Auth.JWT;
using Diquis.Infrastructure.Auth.JWT.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System;
using System.Collections.Generic;

namespace Diquis.Infrastructure.Tests.Auth.JWT
{
    public class TokenServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<IOptions<JWTSettings>> _jwtSettingsOptionsMock;
        private readonly TokenService _tokenService;
        private readonly JWTSettings _jwtSettings;

        public TokenServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(_userManagerMock.Object, contextAccessorMock.Object, userPrincipalFactoryMock.Object, null, null, null, null);

            _jwtSettings = new JWTSettings
            {
                Key = "a-very-long-and-secure-key-for-testing-purposes",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AuthTokenDurationInMinutes = 30,
                RefreshTokenDurationInDays = 14
            };
            _jwtSettingsOptionsMock = new Mock<IOptions<JWTSettings>>();
            _jwtSettingsOptionsMock.Setup(o => o.Value).Returns(_jwtSettings);

            _tokenService = new TokenService(_jwtSettingsOptionsMock.Object, _userManagerMock.Object, _signInManagerMock.Object);
        }

        [Fact]
        public async Task GetTokenAsync_WithValidCredentials_ReturnsTokenResponse()
        {
            // Arrange
            var user = new ApplicationUser
            {
                UserName = "test@test.com",
                Email = "test@test.com",
                EmailConfirmed = true,
                IsActive = true,
                TenantId = "test-tenant"
            };
            var request = new TokenRequest { Email = "test@test.com", Password = "Password123!" };

            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(user.UserName, request.Password, false, false))
                .ReturnsAsync(SignInResult.Success);
             _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "user" });

            // Act
            var result = await _tokenService.GetTokenAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data.Token);
            Assert.NotNull(result.Data.RefreshToken);
        }

        [Fact]
        public async Task GetTokenAsync_WithInvalidUser_ReturnsFailResponse()
        {
            // Arrange
            var request = new TokenRequest { Email = "nouser@test.com", Password = "Password123!" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _tokenService.GetTokenAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid user", result.Messages[0]);
        }

        [Fact]
        public async Task GetTokenAsync_WithDeactivatedUser_ReturnsFailResponse()
        {
            // Arrange
            var user = new ApplicationUser { Email = "deactivated@test.com", IsActive = false };
            var request = new TokenRequest { Email = "deactivated@test.com", Password = "Password123!" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync(user);

            // Act
            var result = await _tokenService.GetTokenAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("User account deactivated", result.Messages[0]);
        }

        [Fact]
        public async Task GetTokenAsync_WithIncorrectPassword_ReturnsFailResponse()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "test@test.com", Email = "test@test.com", EmailConfirmed = true, IsActive = true };
            var request = new TokenRequest { Email = "test@test.com", Password = "WrongPassword" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(user.UserName, request.Password, false, false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _tokenService.GetTokenAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Unauthorized", result.Messages[0]);
        }
        
        [Fact]
        public async Task RefreshTokenAsync_WithValidToken_ReturnsNewToken()
        {
            // Arrange
            var user = new ApplicationUser 
            { 
                UserName = "testuser",
                Email = "testuser@example.com",
                RefreshToken = "valid-token", 
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1), 
                TenantId = "test-tenant" 
            };
            var users = new List<ApplicationUser> { user }.AsQueryable();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(um => um.Users).Returns(users.AsQueryable());
            userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "user" });

            var tokenService = new TokenService(_jwtSettingsOptionsMock.Object, userManagerMock.Object, _signInManagerMock.Object);

            // Act
            var result = await tokenService.RefreshTokenAsync("valid-token");

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data.Token);
        }

        [Fact]
        public async Task RefreshTokenAsync_WithInvalidToken_ReturnsFailResponse()
        {
            // Arrange
             var users = new List<ApplicationUser>().AsQueryable();
            _userManagerMock.Setup(um => um.Users).Returns(users);

            // Act
            var result = await _tokenService.RefreshTokenAsync("invalid-token");

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid token", result.Messages[0]);
        }

        [Fact]
        public async Task RefreshTokenAsync_WithExpiredToken_ReturnsFailResponse()
        {
            // Arrange
            var user = new ApplicationUser { RefreshToken = "expired-token", RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1) };
            var users = new List<ApplicationUser> { user }.AsQueryable();
            
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(um => um.Users).Returns(users.AsQueryable());

            var tokenService = new TokenService(_jwtSettingsOptionsMock.Object, userManagerMock.Object, _signInManagerMock.Object);

            // Act
            var result = await tokenService.RefreshTokenAsync("expired-token");

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Refresh token expired", result.Messages[0]);
        }
    }
}
