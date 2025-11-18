using Diquis.Domain.Entities.Common;

namespace Diquis.Domain.Tests.Entities.Common
{
    /// <summary>
    /// Concrete implementation of <see cref="AuditableEntity"/> for testing purposes.
    /// </summary>
    public class TestAuditableEntity : AuditableEntity { }

    /// <summary>
    /// Contains unit tests for the <see cref="AuditableEntity"/> class and its properties.
    /// </summary>
    public class AuditableEntityTests
    {
        /// <summary>
        /// Tests that the <see cref="AuditableEntity.CreatedBy"/> property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void CanSetAndGetCreatedBy()
        {
            Guid guid = Guid.NewGuid();
            TestAuditableEntity entity = new() { CreatedBy = guid };
            Assert.Equal(guid, entity.CreatedBy);
        }

        /// <summary>
        /// Tests that the <see cref="AuditableEntity.CreatedOn"/> property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void CanSetAndGetCreatedOn()
        {
            DateTime dt = DateTime.UtcNow;
            TestAuditableEntity entity = new() { CreatedOn = dt };
            Assert.Equal(dt, entity.CreatedOn);
        }

        /// <summary>
        /// Tests that the <see cref="AuditableEntity.LastModifiedBy"/> property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void CanSetAndGetLastModifiedBy()
        {
            Guid guid = Guid.NewGuid();
            TestAuditableEntity entity = new() { LastModifiedBy = guid };
            Assert.Equal(guid, entity.LastModifiedBy);
        }

        /// <summary>
        /// Tests that the <see cref="AuditableEntity.LastModifiedOn"/> property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void CanSetAndGetLastModifiedOn()
        {
            DateTime dt = DateTime.UtcNow;
            TestAuditableEntity entity = new() { LastModifiedOn = dt };
            Assert.Equal(dt, entity.LastModifiedOn);
        }

        /// <summary>
        /// Tests that the <see cref="IMustHaveTenant.TenantId"/> property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void CanSetAndGetTenantId()
        {
            TestAuditableEntity entity = new() { TenantId = "tenant-abc" };
            Assert.Equal("tenant-abc", entity.TenantId);
        }

        /// <summary>
        /// Tests that the <see cref="TenantBaseEntity.Id"/> property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void CanSetAndGetId()
        {
            Guid guid = Guid.NewGuid();
            TestAuditableEntity entity = new() { Id = guid };
            Assert.Equal(guid, entity.Id);
        }
    }
}