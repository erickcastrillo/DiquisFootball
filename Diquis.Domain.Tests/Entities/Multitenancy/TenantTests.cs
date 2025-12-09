using System;
using Diquis.Domain.Entities.Multitenancy;
using Diquis.Domain.Enums;
using Xunit;

namespace Diquis.Domain.Tests.Entities.Multitenancy
{
    public class TenantTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesToDefaults()
        {
            Tenant tenant = new()
            {
                Id = "test-id",
                Name = "Test Name"
            };
            Assert.Equal("test-id", tenant.Id);
            Assert.Equal("Test Name", tenant.Name);
            Assert.Null(tenant.ConnectionString);
            Assert.False(tenant.IsActive);
            Assert.Equal(default(DateTime), tenant.CreatedOn);
            Assert.Equal(ProvisioningStatus.Pending, tenant.Status);
        }

        [Fact]
        public void CanSetAndGetProperties()
        {
            DateTime now = DateTime.UtcNow;
            Tenant tenant = new()
            {
                Id = "tenant-1",
                Name = "Test Tenant",
                ConnectionString = "Server=.;Database=Test;Trusted_Connection=True;",
                IsActive = true,
                CreatedOn = now
            };

            Assert.Equal("tenant-1", tenant.Id);
            Assert.Equal("Test Tenant", tenant.Name);
            Assert.Equal("Server=.;Database=Test;Trusted_Connection=True;", tenant.ConnectionString);
            Assert.True(tenant.IsActive);
            Assert.Equal(now, tenant.CreatedOn);
        }

        [Fact]
        public void ConnectionString_NullMeansSharedDatabase()
        {
            Tenant tenant = new()
            {
                Id = "tenant-2",
                Name = "Shared Tenant",
                ConnectionString = null
            };
            Assert.Null(tenant.ConnectionString);
        }
    }
}
