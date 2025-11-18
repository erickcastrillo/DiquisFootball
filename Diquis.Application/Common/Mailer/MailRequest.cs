namespace Diquis.Application.Common.Mailer
{
    public class MailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public string DisplayName { get; set; }
    }
}
