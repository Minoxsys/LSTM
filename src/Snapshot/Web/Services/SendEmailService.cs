﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using Web.Bootstrap;

namespace Web.Services
{
    public class SendEmailService : ISendEmailService
    {
        private string host = AppSettings.SendEmailHost;
        private int port = Int32.Parse(AppSettings.SendEmailPort);

        public bool SendMail(System.Net.Mail.MailMessage message)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.Host = host;
                client.Port = port;
                client.Send(message);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}