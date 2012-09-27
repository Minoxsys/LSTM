using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using System.Net.Mail;
using Core.Persistence;
using Web.Bootstrap;

namespace Web.Services
{
    public class EmailMessageService : IEmailMessageService
    {
        private IQueryService<Outpost> queryServiceOutpost;
        private ISendEmailService emailService;

        public EmailMessageService(IQueryService<Outpost> queryServiceOutpost, ISendEmailService emailService)
        {
            this.queryServiceOutpost = queryServiceOutpost;
            this.emailService = emailService;
        }

        public bool SendEmail(RawSmsReceived rawSmsReceived)
        {
            MailMessage mail = new MailMessage();

            mail.To.Add(new MailAddress(AppSettings.SendEmailTo));
            mail.CC.Add(new MailAddress(AppSettings.SendEmailCC));
            mail.From = new MailAddress(AppSettings.SendEmailFrom);

            mail.Subject = "Incorrect message format";
            mail.IsBodyHtml = true;
            mail.Body = CreateBodyMessage(rawSmsReceived);

            return (emailService.SendMail(mail) == "Email has been sent");
        }

        private string CreateBodyMessage(RawSmsReceived rawSmsReceived)
        {
            string outpost = " ";
            if (rawSmsReceived.OutpostId != Guid.Empty)
                outpost = queryServiceOutpost.Load(rawSmsReceived.OutpostId).Name;

            string body = "Dear  John <br/>";
            body += "The following health facility (" + outpost + ", " + rawSmsReceived.Sender + ") is having difficulty in supplying their information. They have tried unsuccessfully 3 times.<br/>";
            body += "This situation needs to be rectified as soon as possible please.<br/>";
            body += "Regards,<br/>";
            body += "The IntHEC Support Team.";

            return body;
        }
    }
}