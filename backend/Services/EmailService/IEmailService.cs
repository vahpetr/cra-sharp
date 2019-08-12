using System.Threading;
using System.Threading.Tasks;

namespace Backend.Services.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message, CancellationToken ct);
    }
}