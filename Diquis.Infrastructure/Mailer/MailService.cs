using Diquis.Application.Common.Mailer;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;


namespace Diquis.Infrastructure.Mailer
{
    /// <summary>
    /// Provides email sending functionality using MailKit.
    /// </summary>
    public class MailService : IMailService // mailer implentation with MailKit (3rd-party package)
    {
        private readonly MailSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailService"/> class.
        /// </summary>
        /// <param name="settings">The mail settings options.</param>
        public MailService(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="request">The mail request details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendAsync(MailRequest request)
        {
            // create message
            MimeMessage email = new();
            email.From.Add(new MailboxAddress(_settings.DisplayName, request.From ?? _settings.From));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;

            BodyBuilder builder = new()
            {
                HtmlBody = request.Body
            };
            email.Body = builder.ToMessageBody();

            // send email
            using SmtpClient smtp = new();
            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.UserName, _settings.Password);
            _ = await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
