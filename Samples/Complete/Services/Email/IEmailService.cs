using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MvcSample.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage emailMessage);
    }
}
