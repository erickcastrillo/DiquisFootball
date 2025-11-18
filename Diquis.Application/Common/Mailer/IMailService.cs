using Diquis.Application.Common.Marker;

namespace Diquis.Application.Common.Mailer
{
    /// <summary>
    /// Service interface for sending emails.
    /// </summary>
    public interface IMailService : IScopedService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="request">The mail request details.</param>
        Task SendAsync(MailRequest request);
    }
}
