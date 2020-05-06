using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Sample.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage emailMessage);
    }
}
