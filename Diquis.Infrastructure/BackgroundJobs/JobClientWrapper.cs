using System.Linq.Expressions;
using Hangfire;

namespace Diquis.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// A wrapper for the Hangfire IBackgroundJobClient to allow for easier testing and dependency injection.
    /// </summary>
    public class JobClientWrapper : IJobClientWrapper
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobClientWrapper"/> class.
        /// </summary>
        /// <param name="backgroundJobClient">The Hangfire background job client.</param>
        public JobClientWrapper(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        /// <inheritdoc/>
        public string Enqueue(Expression<Action> methodCall)
        {
            return _backgroundJobClient.Enqueue(methodCall);
        }
    }
}