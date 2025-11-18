using System;
using Diquis.Domain.Entities.Common;
using Xunit;

namespace Diquis.Domain.Tests.Entities.Common
{
    /// <summary>
    /// Unit tests for the <see cref="IAuditableEntity"/> contract verifying property getters and setters.
    /// </summary>
    public class IAuditableEntityTests
    {
        // Minimal concrete implementation for testing the interface contract.
        private class TestAuditableEntity : IAuditableEntity
        {
            public Guid CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
            public Guid? LastModifiedBy { get; set; }
            public DateTime? LastModifiedOn { get; set; }
        }

        [Fact]
        public void CanSetAndGetCreatedBy()
        {
            Guid guid = Guid.NewGuid();
            TestAuditableEntity entity = new() { CreatedBy = guid };
            Assert.Equal(guid, entity.CreatedBy);
        }

        [Fact]
        public void CanSetAndGetCreatedOn()
        {
            DateTime now = DateTime.UtcNow;
            TestAuditableEntity entity = new() { CreatedOn = now };
            Assert.Equal(now, entity.CreatedOn);
        }

        [Fact]
        public void CanSetAndGetLastModifiedBy()
        {
            Guid guid = Guid.NewGuid();
            TestAuditableEntity entity = new() { LastModifiedBy = guid };
            Assert.Equal(guid, entity.LastModifiedBy);
        }

        [Fact]
        public void CanSetAndGetLastModifiedOn()
        {
            DateTime now = DateTime.UtcNow;
            TestAuditableEntity entity = new() { LastModifiedOn = now };
            Assert.Equal(now, entity.LastModifiedOn);
        }
    }
}