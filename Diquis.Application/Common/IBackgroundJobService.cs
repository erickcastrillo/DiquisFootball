using System.Linq.Expressions;

namespace Diquis.Application.Common;

/// <summary>
/// Service interface for background job scheduling.
/// </summary>
public interface IBackgroundJobService
{
    /// <summary>
    /// Enqueues a background job for execution.
    /// </summary>
    /// <param name="methodCall">The method call expression to enqueue.</param>
    /// <returns>The job identifier.</returns>
    string Enqueue(Expression<Action> methodCall);
}
