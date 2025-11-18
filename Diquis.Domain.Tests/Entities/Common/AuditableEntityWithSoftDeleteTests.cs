using System;
using Diquis.Domain.Entities.Common;
using Xunit;

namespace Diquis.Domain.Tests.Entities.Common
{
    /// <summary>
    /// Unit tests for the <see cref="AuditableEntityWithSoftDelete"/> properties.
    /// A concrete test subclass is used because the target type is abstract.
    /// </summary>
    public class AuditableEntityWithSoftDeleteTests
    {
        private class TestEntity : AuditableEntityWithSoftDelete { }

        [Fact]
        public void CanSetAndGetCreatedBy()
        {
            Guid guid = Guid.NewGuid();
            TestEntity e = new() { CreatedBy = guid };
            Assert.Equal(guid, e.CreatedBy);
        }

        [Fact]
        public void CanSetAndGetCreatedOn()
        {
            DateTime dt = DateTime.UtcNow;
            TestEntity e = new() { CreatedOn = dt };
            Assert.Equal(dt, e.CreatedOn);
        }

        [Fact]
        public void CanSetAndGetLastModifiedBy()
        {
            Guid guid = Guid.NewGuid();
            TestEntity e = new() { LastModifiedBy = guid };
            Assert.Equal(guid, e.LastModifiedBy);
        }

        [Fact]
        public void CanSetAndGetLastModifiedOn()
        {
            DateTime dt = DateTime.UtcNow;
            TestEntity e = new() { LastModifiedOn = dt };
            Assert.Equal(dt, e.LastModifiedOn);
        }

        [Fact]
        public void CanSetAndGetDeletedBy()
        {
            Guid guid = Guid.NewGuid();
            TestEntity e = new() { DeletedBy = guid };
            Assert.Equal(guid, e.DeletedBy);
        }

        [Fact]
        public void CanSetAndGetDeletedOn()
        {
            DateTime dt = DateTime.UtcNow;
            TestEntity e = new() { DeletedOn = dt };
            Assert.Equal(dt, e.DeletedOn);
        }

        [Fact]
        public void CanSetAndGetTenantId()
        {
            TestEntity e = new() { TenantId = "tenant-xyz" };
            Assert.Equal("tenant-xyz", e.TenantId);
        }

        [Fact]
        public void CanSetAndGetId()
        {
            Guid id = Guid.NewGuid();
            TestEntity e = new() { Id = id };
            Assert.Equal(id, e.Id);
        }
    }
}