using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace IdentityExample.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } 

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(subject, message, email, Options.SendGridKey, Options.SenderName, Options.SenderEmail, Options.RecipientName);
            //return TestExecute();
        }

        public async Task Execute(string subject, string message, string receiverEmail, string apiKey, string senderName, string senderEmail, string recipientName = "User")
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(senderEmail, senderName);
            var to = new EmailAddress(receiverEmail, recipientName);
            var plainTextContent = message;
            var htmlContent = message;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
