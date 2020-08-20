namespace NeverFoundry.Wiki.Samples.Complete.Services
{
    public interface IEmailConfiguration
    {
        string FromAddress { get; set; }
        string? FromName { get; set; }
        string? ReplyToAddress { get; set; }

        string? SmtpPassword { get; set; }
        int SmtpPort { get; }
        string? SmtpServer { get; }
        string? SmtpUsername { get; set; }
        bool SmtpUseSSL { get; set; }
    }
}
