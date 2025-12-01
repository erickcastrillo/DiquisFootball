using System.Linq.Expressions;

namespace Diquis.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Defines a wrapper for a background job client to allow for easier testing and dependency injection.
    /// </summary>
    public interface IJobClientWrapper
    {
        /// <summary>
        /// Enqueues a background job.
        /// </summary>
        /// <param name="methodCall">The method call expression to enqueue.</param>
        /// <returns>The ID of the enqueued job.</returns>
        string Enqueue(Expression<Action> methodCall);
    }
}
