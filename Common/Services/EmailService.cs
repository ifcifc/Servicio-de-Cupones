using Common.Interfaces;
using Common.Models.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class EmailService : IEmailService
    {
        private SmtpConfig smtpConfig { get; set; }

        public EmailService(SmtpConfig smtpConfig)
        {
            this.smtpConfig = smtpConfig;
        }

        public async Task EnviarEmail(string email, string subject, string message)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(this.smtpConfig.Host);
                smtpClient.Port = this.smtpConfig.Port;
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(this.smtpConfig.Email, this.smtpConfig.Password);

                MailMessage mailMessage = new MailMessage();
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress("ProgramacionIV@testing.com", "ProgramacionIV");
                mailMessage.Subject = subject;
                mailMessage.Body = message;
                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n" + "Verifique la configuracion del servidor SMTP.");// this.smtpConfig.ToString()
            }
        }
    }
}
