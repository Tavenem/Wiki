using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Complete.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage emailMessage);
    }
}
