using System.Linq.Expressions;
using Diquis.Application.Common;

namespace Diquis.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Service for enqueuing background jobs using a job client wrapper.
    /// </summary>
    public class HangfireJobService : IBackgroundJobService
    {
        private readonly IJobClientWrapper _jobClientWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="HangfireJobService"/> class.
        /// </summary>
        /// <param name="jobClientWrapper">The job client wrapper to use for enqueuing jobs.</param>
        public HangfireJobService(IJobClientWrapper jobClientWrapper)
        {
            _jobClientWrapper = jobClientWrapper;
        }

        /// <inheritdoc/>
        public string Enqueue(Expression<Action> methodCall)
        {
            return _jobClientWrapper.Enqueue(methodCall);
        }
    }
}
