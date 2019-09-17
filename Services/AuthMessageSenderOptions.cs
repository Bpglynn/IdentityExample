using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityExample.Services
{
    public class AuthMessageSenderOptions
    {
        // SendGrid settings
        public string SendGridKey { get; set; }
        // SMTP settings
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string Password { get; set; }
        // Who generated e-mails should be from
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        // How to address receiver of generated e-mails
        public string RecipientName { get; set; }
    }
}
