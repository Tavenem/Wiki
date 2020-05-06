using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Sample.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IEmailConfiguration emailConfiguration, ILogger<EmailService> logger)
        {
            _emailConfiguration = emailConfiguration;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            if (string.IsNullOrEmpty(_emailConfiguration.SmtpServer))
            {
                _logger.LogError("Email Send Failed: {0}", "SMTP server not configured");
                throw new Exception("SMTP server not configured");
            }

            try
            {
                var message = new MimeMessage();

                // Set From Address if it was not set
                if (emailMessage.FromAddresses.Count == 0)
                {
                    emailMessage.FromAddresses.Add(new EmailAddress(_emailConfiguration.FromAddress, _emailConfiguration.FromName));
                }

                message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
                message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
                message.Cc.AddRange(emailMessage.CcAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
                message.Bcc.AddRange(emailMessage.BccAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

                message.Subject = emailMessage.Subject;

                message.Body = emailMessage.IsHtml ? new BodyBuilder { HtmlBody = emailMessage.Body }.ToMessageBody() : new TextPart("plain") { Text = emailMessage.Body };

                using var emailClient = new SmtpClient();
                if (!_emailConfiguration.SmtpUseSSL)
                {
                    emailClient.ServerCertificateValidationCallback = (object _, X509Certificate? __, X509Chain? ___, SslPolicyErrors ____) => true;
                }

                await emailClient.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, _emailConfiguration.SmtpUseSSL).ConfigureAwait(false);

                //Remove any OAuth functionality as we won't be using it.
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                if (!string.IsNullOrWhiteSpace(_emailConfiguration.SmtpUsername))
                {
                    await emailClient.AuthenticateAsync(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword).ConfigureAwait(false);
                }

                await emailClient.SendAsync(message).ConfigureAwait(false);

                await emailClient.DisconnectAsync(true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError("Email Send Failed: {0}", ex.Message);
#if DEBUG
                throw new Exception(ex.Message);
#else
                throw new Exception();
#endif
            }
        }
    }
}
