namespace Diquis.Infrastructure.Mailer
{
    /// <summary>
    /// Represents the settings required for sending emails.
    /// </summary>
    public class MailSettings
    {
        /// <summary>
        /// Gets or sets the sender's email address.
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// Gets or sets the SMTP host.
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Gets or sets the SMTP port.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Gets or sets the username for SMTP authentication.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the password for SMTP authentication.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the display name for the sender.
        /// </summary>
        public string DisplayName { get; set; }
    }
}
