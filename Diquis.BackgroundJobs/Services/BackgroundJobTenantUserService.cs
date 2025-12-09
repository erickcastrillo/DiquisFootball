using Diquis.Application.Common;

namespace Diquis.BackgroundJobs.Services;

/// <summary>
/// Implementation of <see cref="ICurrentTenantUserService"/> for background jobs.
/// Background jobs don't have a user context, so this returns empty values.
/// </summary>
public class BackgroundJobTenantUserService : ICurrentTenantUserService
{
    public string TenantId => string.Empty;
    public string UserId => string.Empty;
    public string ConnectionString => string.Empty;

    string? ICurrentTenantUserService.ConnectionString { get => ConnectionString; set => throw new NotImplementedException(); }
    string? ICurrentTenantUserService.TenantId { get => TenantId; set => throw new NotImplementedException(); }
    string? ICurrentTenantUserService.UserId { get => UserId; set => throw new NotImplementedException(); }

    public Task<bool> SetTenantUser(string tenant) => throw new NotImplementedException();
}
