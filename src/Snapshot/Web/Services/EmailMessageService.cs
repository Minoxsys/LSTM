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

            mail.Subject = "Ongezeko la muundo usio sahihi";
            mail.IsBodyHtml = true;
            mail.Body = CreateBodyMessage(rawSmsReceived);

            return (emailService.SendMail(mail) == "Email has been sent");
        }

        private string CreateBodyMessage(RawSmsReceived rawSmsReceived)
        {
            string outpost = " ";
            if (rawSmsReceived.OutpostId != Guid.Empty)
                outpost = queryServiceOutpost.Load(rawSmsReceived.OutpostId).Name;

            string body = "Ndugu John <br/>";
            body += "Alama maalum  ya siri inayofuata (" + outpost + ", " + rawSmsReceived.Sender + ") ina matatizo katika kugawa mawasiliano.Wamejaribu mara tatu bila mafanikio. <br/>";
            body += "Hali hii inahitaji kurekebishwa haraka iwezekanavyo.<br/>";
            body += "Wako,<br/>";
            body += "Jopo  la wasaidizi  la  intHEC.";

            return body;
        }
    }
}