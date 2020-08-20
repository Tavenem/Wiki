using System.Collections.Generic;

namespace NeverFoundry.Wiki.Samples.Complete.Services
{
    public class EmailMessage
    {
        public string? Body { get; set; }
        public bool IsHtml { get; set; } = true;
        public string? Subject { get; set; }

        public List<EmailAddress> BccAddresses { get; set; } = new List<EmailAddress>();
        public List<EmailAddress> CcAddresses { get; set; } = new List<EmailAddress>();
        public List<EmailAddress> FromAddresses { get; set; } = new List<EmailAddress>();
        public List<EmailAddress> ToAddresses { get; set; } = new List<EmailAddress>();
    }
}
