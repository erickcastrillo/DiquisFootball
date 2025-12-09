namespace Diquis.Application.Common.Mailer
{
    /// <summary>
    /// Represents a request to send an email.
    /// </summary>
    public class MailRequest
    {
        /// <summary>
        /// Gets or sets the recipient's email address.
        /// </summary>
        public string To { get; set; } = null!;
        /// <summary>
        /// Gets or sets the subject of the email.
        /// </summary>
        public string Subject { get; set; } = null!;
        /// <summary>
        /// Gets or sets the body of the email.
        /// </summary>
        public string Body { get; set; } = null!;
        /// <summary>
        /// Gets or sets the sender's email address.
        /// </summary>
        public string From { get; set; } = null!;
        /// <summary>
        /// Gets or sets the display name of the sender.
        /// </summary>
        public string DisplayName { get; set; } = null!;
    }
}
