using System.Linq.Expressions;

namespace Diquis.Application.Common.BackgroundJobs;

/// <summary>
/// Defines a wrapper for a background job client to allow for easier testing and dependency injection.
/// This abstraction allows the application layer to enqueue jobs without depending on specific infrastructure implementations.
/// </summary>
public interface IJobClientWrapper
{
    /// <summary>
    /// Enqueues a background job for execution.
    /// </summary>
    /// <param name="methodCall">The method call expression to enqueue.</param>
    /// <returns>The unique identifier of the enqueued job.</returns>
    string Enqueue(Expression<Action> methodCall);
}
