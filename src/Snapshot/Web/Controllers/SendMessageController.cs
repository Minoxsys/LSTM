using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.Shared;
using Web.Services;
using Web.Bootstrap;
using System.Net;
using System.IO;
using System.Net.Mail;
using System.Data.SqlClient;
using FluentNHibernate.Cfg.Db;
using Web.Models.SendMessage;
using System.Security.AccessControl;
using System.Web.Hosting;
using System.Security.Principal;

namespace Web.Controllers
{
    public class SendMessageController : Controller
    {
        public ISmsGatewayService smsGatewayService { get; set; }
        public IEmailService EmailService { get; set; }
        public IFileService FileService { get; set; }
        public IHttpService HttpService { get; set; }

        public ActionResult Overview()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Send(string phoneNumber, string message, string gateway)
        {
            string url = @"http://localhost:53692/SmsRequest/ReceiveSms?";

            if (gateway == "remote")
                url = AppSettings.SmsGatewayUrl + "?phonenumber=%2B" + phoneNumber + "&user=" + AppSettings.SmsGatewayUserName + "&password=" + AppSettings.SmsGatewayPassword + "&text=" + Url.Encode(message);
            else
                url += "?message=" + message + "&msisdn=" + phoneNumber;
   
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            var result = "";
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                sr.Close();
            }

            return Json(
                   new JsonActionResponse
                   {
                       Status = response.StatusCode.ToString(),
                       Message = result
                   });
                        
        }

        [HttpPost]
        public JsonResult SendEmail(string message)
        {
            MailMessage mail = new MailMessage();

            mail.To.Add(new MailAddress("be_juggernant@yahoo.com"));

            mail.From = new MailAddress("snapshot@evozon.com");
            mail.Subject = "Test";
            mail.IsBodyHtml = false;
            mail.Body = "This is a test";

            var ok = EmailService.SendMail(mail);

            return Json(
                   new JsonActionResponse
                   {
                       Status = ok.ToString(),
                       Message = ok.ToString()
                   });
        
        }
    }
}
