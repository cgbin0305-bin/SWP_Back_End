using API.Interfaces;
using EASendMail;
using Microsoft.Extensions.Options;
using MimeKit;


/*
download 3 package:
 EASendMail
 MailKit
 MimeKit 
*/
namespace API.Services
{
    public class SendMailService : ISendMailService
    {
        private readonly MailSettings _mailSettings;
        public SendMailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task SendMailAsync(MailContent mailContent)
        {
            //1. create mail
            var email = new MimeMessage();
            //1.1 create sender
            email.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
            //1.2 create who send mail
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            //1.3 create who receive email
            email.To.Add(new MailboxAddress(mailContent.To, mailContent.To));
            //1.4 create subject
            email.Subject = mailContent.Subject;
            //1.5 create body message
            var builder = new BodyBuilder();
            builder.HtmlBody = mailContent.Body;
            email.Body = builder.ToMessageBody();

            //2. create smtp
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                //2.1 connect
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                //2.2 authenticate
                await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);

                //2.3 send email
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                Console.WriteLine("Email sent successfully.");

            }
            catch (System.Exception ex)
            {
                throw new Exception($"Email sending failed: {ex.Message}");
            }
        }
    }
}