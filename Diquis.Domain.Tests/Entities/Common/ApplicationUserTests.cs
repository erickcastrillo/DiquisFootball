using Diquis.Domain.Entities.Common;

namespace Diquis.Domain.Tests.Entities.Common
{
    /// <summary>
    /// Contains unit tests for the <see cref="ApplicationUser"/> class, verifying property setters and getters.
    /// </summary>
    public class ApplicationUserTests
    {
        /// <summary>
        /// Tests that the <see cref="ApplicationUser.FirstName"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetFirstName()
        {
            ApplicationUser user = new() { FirstName = "Alice" };
            Assert.Equal("Alice", user.FirstName);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.LastName"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetLastName()
        {
            ApplicationUser user = new() { LastName = "Smith" };
            Assert.Equal("Smith", user.LastName);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.TenantId"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetTenantId()
        {
            ApplicationUser user = new() { TenantId = "tenant-123" };
            Assert.Equal("tenant-123", user.TenantId);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.IsActive"/> property defaults to true.
        /// </summary>
        [Fact]
        public void IsActiveDefaultsToTrue()
        {
            ApplicationUser user = new();
            Assert.True(user.IsActive);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.IsActive"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetIsActive()
        {
            ApplicationUser user = new() { IsActive = false };
            Assert.False(user.IsActive);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.ImageUrl"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetImageUrl()
        {
            ApplicationUser user = new() { ImageUrl = "http://img.com/pic.png" };
            Assert.Equal("http://img.com/pic.png", user.ImageUrl);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.RefreshToken"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetRefreshToken()
        {
            ApplicationUser user = new() { RefreshToken = "token123" };
            Assert.Equal("token123", user.RefreshToken);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.RefreshTokenExpiryTime"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetRefreshTokenExpiryTime()
        {
            DateTime now = DateTime.UtcNow;
            ApplicationUser user = new() { RefreshTokenExpiryTime = now };
            Assert.Equal(now, user.RefreshTokenExpiryTime);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.RoleId"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetRoleId()
        {
            ApplicationUser user = new() { RoleId = "role-xyz" };
            Assert.Equal("role-xyz", user.RoleId);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.CreatedBy"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetCreatedBy()
        {
            Guid guid = Guid.NewGuid();
            ApplicationUser user = new() { CreatedBy = guid };
            Assert.Equal(guid, user.CreatedBy);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.CreatedOn"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetCreatedOn()
        {
            DateTime dt = DateTime.UtcNow;
            ApplicationUser user = new() { CreatedOn = dt };
            Assert.Equal(dt, user.CreatedOn);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.LastModifiedBy"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetLastModifiedBy()
        {
            Guid guid = Guid.NewGuid();
            ApplicationUser user = new() { LastModifiedBy = guid };
            Assert.Equal(guid, user.LastModifiedBy);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.LastModifiedOn"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetLastModifiedOn()
        {
            DateTime dt = DateTime.UtcNow;
            ApplicationUser user = new() { LastModifiedOn = dt };
            Assert.Equal(dt, user.LastModifiedOn);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.DeletedBy"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetDeletedBy()
        {
            Guid guid = Guid.NewGuid();
            ApplicationUser user = new() { DeletedBy = guid };
            Assert.Equal(guid, user.DeletedBy);
        }

        /// <summary>
        /// Tests that the <see cref="ApplicationUser.DeletedOn"/> property can be set and retrieved.
        /// </summary>
        [Fact]
        public void CanSetAndGetDeletedOn()
        {
            DateTime dt = DateTime.UtcNow;
            ApplicationUser user = new() { DeletedOn = dt };
            Assert.Equal(dt, user.DeletedOn);
        }
    }
}