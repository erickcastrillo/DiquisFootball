using AutoMapper;
using Diquis.Application.Common;
using Diquis.Application.Common.Identity;
using Diquis.Application.Common.Identity.DTOs;
using Diquis.Application.Common.Images;
using Diquis.Application.Common.Mailer;
using Diquis.Domain.Entities.Common;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Diquis.Infrastructure.Identity;

namespace Diquis.Infrastructure.Tests.Identity
{
    public class IdentityServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<ICurrentTenantUserService> _currentTenantUserServiceMock;
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<IImageService> _imageServiceMock;
        private readonly IdentityService _identityService;

        public IdentityServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _currentTenantUserServiceMock = new Mock<ICurrentTenantUserService>();
            _mailServiceMock = new Mock<IMailService>();
            _imageServiceMock = new Mock<IImageService>();

            _identityService = new IdentityService(
                _mapperMock.Object,
                _userManagerMock.Object,
                _currentTenantUserServiceMock.Object,
                _mailServiceMock.Object,
                _imageServiceMock.Object);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsListOfUsers()
        {
            // Arrange
            var users = new List<ApplicationUser> { new ApplicationUser(), new ApplicationUser() };
            var userDtos = new List<UserDto> { new UserDto(), new UserDto() };
            
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            
            var usersQueryable = users.AsQueryable();
            var mockUserSet = new Mock<DbSet<ApplicationUser>>();
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(usersQueryable.Provider);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(usersQueryable.Expression);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(usersQueryable.ElementType);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(usersQueryable.GetEnumerator());
            
            userManagerMock.Setup(um => um.Users).Returns(mockUserSet.Object);
            userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "user" });

            _mapperMock.Setup(m => m.Map<List<UserDto>>(It.IsAny<List<ApplicationUser>>())).Returns(userDtos);
            
            var identityService = new IdentityService(_mapperMock.Object, userManagerMock.Object, _currentTenantUserServiceMock.Object, _mailServiceMock.Object, _imageServiceMock.Object);

            // Act
            var result = await identityService.GetUsersAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task GetUserDetailsAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId.ToString() };
            var userDto = new UserDto { Id = userId };
             var users = new List<ApplicationUser> { user }.AsQueryable();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            var mockUserSet = new Mock<DbSet<ApplicationUser>>();
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            userManagerMock.Setup(um => um.Users).Returns(mockUserSet.Object);
            userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "user" });
            _mapperMock.Setup(m => m.Map(user, It.IsAny<UserDto>())).Returns(userDto);
            
            var identityService = new IdentityService(_mapperMock.Object, userManagerMock.Object, _currentTenantUserServiceMock.Object, _mailServiceMock.Object, _imageServiceMock.Object);

            // Act
            var result = await identityService.GetUserDetailsAsync(userId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(userId, result.Data.Id);
        }

        [Fact]
        public async Task GetUserDetailsAsync_WhenUserDoesNotExist_ReturnsFailResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var users = new List<ApplicationUser>().AsQueryable();
             var userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            var mockUserSet = new Mock<DbSet<ApplicationUser>>();
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());
            userManagerMock.Setup(um => um.Users).Returns(mockUserSet.Object);
             var identityService = new IdentityService(_mapperMock.Object, userManagerMock.Object, _currentTenantUserServiceMock.Object, _mailServiceMock.Object, _imageServiceMock.Object);


            // Act
            var result = await identityService.GetUserDetailsAsync(userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("User doesn't exist", result.Messages[0]);
        }

        [Fact]
        public async Task RegisterUserAsync_WithNewUser_ReturnsSuccess()
        {
            // Arrange
            var request = new RegisterUserRequest { Email = "new@test.com", Password = "Password123!", RoleId = "user" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), request.Password)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), request.RoleId)).ReturnsAsync(IdentityResult.Success);
            _mapperMock.Setup(m => m.Map(It.IsAny<ApplicationUser>(), It.IsAny<UserDto>())).Returns(new UserDto());

            // Act
            var result = await _identityService.RegisterUserAsync(request);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task RegisterUserAsync_WithExistingUser_ReturnsFail()
        {
            // Arrange
            var request = new RegisterUserRequest { Email = "existing@test.com" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync(new ApplicationUser());

            // Act
            var result = await _identityService.RegisterUserAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("User already exists", result.Messages[0]);
        }

        [Fact]
        public async Task RegisterUserAsync_WithFailedCreation_ReturnsFail()
        {
            // Arrange
            var request = new RegisterUserRequest { Email = "new@test.com", Password = "Password123!" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), request.Password)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Creation failed" }));

            // Act
            var result = await _identityService.RegisterUserAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Creation failed", result.Messages[0]);
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateUserRequest { Email = "updated@test.com", RoleId = "admin" };
            var user = new ApplicationUser { Id = userId.ToString(), Email = "original@test.com" };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "user" });
            _userManagerMock.Setup(um => um.RemoveFromRolesAsync(user, It.IsAny<string[]>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(user, request.RoleId)).ReturnsAsync(IdentityResult.Success);
            _mapperMock.Setup(m => m.Map(request, user)).Returns(user);
            _mapperMock.Setup(m => m.Map(user, It.IsAny<UserDto>())).Returns(new UserDto());
            
            // Act
            var result = await _identityService.UpdateUserAsync(request, userId);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task UpdateUserAsync_WithNonExistentUser_ReturnsFail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateUserRequest();
            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _identityService.UpdateUserAsync(request, userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Not found", result.Messages[0]);
        }
        
        [Fact]
        public async Task UpdateUserAsync_WithExistingEmail_ReturnsFail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateUserRequest { Email = "existing@test.com" };
            var user = new ApplicationUser { Id = userId.ToString(), Email = "original@test.com" };
            var otherUser = new ApplicationUser { Id = Guid.NewGuid().ToString(), Email = "existing@test.com" };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync(otherUser);

            // Act
            var result = await _identityService.UpdateUserAsync(request, userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Email already in use", result.Messages[0]);
        }

        [Fact]
        public async Task UpdateUserAsync_CannotEditRootUser_ReturnsFail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateUserRequest();
            var user = new ApplicationUser { Id = userId.ToString() };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "root" });

            // Act
            var result = await _identityService.UpdateUserAsync(request, userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Cannot edit root user", result.Messages[0]);
        }

        [Fact]
        public async Task UpdateUserAsync_CannotUpdateCurrentUser_ReturnsFail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateUserRequest();
            var user = new ApplicationUser { Id = userId.ToString() };

            _currentTenantUserServiceMock.Setup(s => s.UserId).Returns(userId.ToString());
            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "user" });

            // Act
            var result = await _identityService.UpdateUserAsync(request, userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Cannot update current user", result.Messages[0]);
        }

        [Fact]
        public async Task DeleteUserAsync_WithExistingUser_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId.ToString() };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "user" });
            _userManagerMock.Setup(um => um.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _identityService.DeleteUserAsync(userId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(userId, result.Data);
        }

        [Fact]
        public async Task DeleteUserAsync_WithNonExistentUser_ReturnsFail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _identityService.DeleteUserAsync(userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Not found", result.Messages[0]);
        }

        [Fact]
        public async Task DeleteUserAsync_CannotDeleteRootUser_ReturnsFail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId.ToString() };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "root" });

            // Act
            var result = await _identityService.DeleteUserAsync(userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Cannot delete root user", result.Messages[0]);
        }

        [Fact]
        public async Task DeleteUserAsync_CannotDeleteCurrentUser_ReturnsFail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId.ToString() };
            _currentTenantUserServiceMock.Setup(s => s.UserId).Returns(userId.ToString());
            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
             _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "user" });

            // Act
            var result = await _identityService.DeleteUserAsync(userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Cannot delete current user", result.Messages[0]);
        }

        [Fact]
        public async Task GetProfileDetailsAsync_ReturnsCurrentUserProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId.ToString() };
            var userDto = new UserDto { Id = userId };
             var users = new List<ApplicationUser> { user }.AsQueryable();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            var mockUserSet = new Mock<DbSet<ApplicationUser>>();
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            userManagerMock.Setup(um => um.Users).Returns(mockUserSet.Object);
            userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "user" });
            _mapperMock.Setup(m => m.Map(user, It.IsAny<UserDto>())).Returns(userDto);
            _currentTenantUserServiceMock.Setup(s => s.UserId).Returns(userId.ToString());
            
            var identityService = new IdentityService(_mapperMock.Object, userManagerMock.Object, _currentTenantUserServiceMock.Object, _mailServiceMock.Object, _imageServiceMock.Object);


            // Act
            var result = await identityService.GetProfileDetailsAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(userId, result.Data.Id);
        }

        [Fact]
        public async Task ChangePasswordAsync_WithValidPassword_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new ApplicationUser { Id = userId };
            var request = new ChangePasswordRequest { Password = "old", NewPassword = "new" };
            _currentTenantUserServiceMock.Setup(s => s.UserId).Returns(userId);
            _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.ChangePasswordAsync(user, request.Password, request.NewPassword)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _identityService.ChangePasswordAsync(request);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task ForgotPasswordAsync_WithValidEmail_SendsEmail()
        {
            // Arrange
            var request = new ForgotPasswordRequest { Email = "test@test.com" };
            var user = new ApplicationUser { Email = request.Email, IsActive = true };
            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email.Normalize())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("token");

            // Act
            var result = await _identityService.ForgotPasswordAsync(request, "origin", "route");

            // Assert
            Assert.True(result.Succeeded);
            _mailServiceMock.Verify(m => m.SendAsync(It.IsAny<MailRequest>()), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordAsync_WithValidToken_ReturnsSuccess()
        {
            // Arrange
            var request = new ResetPasswordRequest { Email = "test@test.com", Token = "token", Password = "new" };
            var user = new ApplicationUser { Email = request.Email, IsActive = true };
            _userManagerMock.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.ResetPasswordAsync(user, request.Token, request.Password)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _identityService.ResetPasswordAsync(request);

            // Assert
            Assert.True(result.Succeeded);
        }
    }
}
