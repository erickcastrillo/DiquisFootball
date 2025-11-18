using System;
using System.Linq.Expressions;
using Hangfire;

namespace Diquis.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Provides an abstraction for enqueuing background jobs.
    /// </summary>
    public interface IJobClientWrapper
    {
        /// <summary>
        /// Enqueues a background job for execution.
        /// </summary>
        /// <param name="methodCall">The method call expression to enqueue.</param>
        /// <returns>The job identifier.</returns>
        string Enqueue(Expression<Action> methodCall);
    }

    /// <summary>
    /// Implementation of <see cref="IJobClientWrapper"/> that delegates to Hangfire's <see cref="IBackgroundJobClient"/>.
    /// </summary>
    public class JobClientWrapper : IJobClientWrapper
    {
        private readonly IBackgroundJobClient _jobClient;
        /// <summary>
        /// Initializes a new instance of the <see cref="JobClientWrapper"/> class.
        /// </summary>
        /// <param name="jobClient">The Hangfire background job client.</param>
        public JobClientWrapper(IBackgroundJobClient jobClient)
        {
            _jobClient = jobClient;
        }
        /// <inheritdoc/>
        public string Enqueue(Expression<Action> methodCall)
        {
            return _jobClient.Enqueue(methodCall);
        }
    }
}
