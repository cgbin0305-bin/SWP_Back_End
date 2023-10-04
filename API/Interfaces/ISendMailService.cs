
using API.Services;

namespace API.Interfaces
{
    public interface ISendMailService
    {
        Task SendMailAsync(MailContent mailContent);
    }
}