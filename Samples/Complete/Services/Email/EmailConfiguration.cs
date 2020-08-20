namespace NeverFoundry.Wiki.MvcSample.Services
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public string FromAddress { get; set; } = "example@example.com";
        public string? FromName { get; set; }
        public string? ReplyToAddress { get; set; }

        public string? SmtpPassword { get; set; }
        public int SmtpPort { get; } = 465;
        public string? SmtpServer { get; set; }
        public string? SmtpUsername { get; set; }
        public bool SmtpUseSSL { get; set; } = true;
    }
}
