﻿using System;
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
using Domain;
using Core.Persistence;
using Web.Security;

namespace Web.Controllers
{
    public class SendMessageController : Controller
    {
        public ISmsGatewayService smsGatewayService { get; set; }
        public IEmailService EmailService { get; set; }
        public IFileService FileService { get; set; }
        public IHttpService HttpService { get; set; }
        public ISaveOrUpdateCommand<SentSms> SaveOrUpdateCommand { get; set; }

        [Requires(Permissions = "Client.View")]
        public ActionResult Overview()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Send(string phoneNumber, string message, string gateway)
        {
            if (gateway == "remote")
            {
                string postData = GeneratePostData(message, phoneNumber);
                string responseString = "";

                try
                {
                    responseString = HttpService.Post(postData);
                    SaveMessage("+" + phoneNumber, message, responseString);

                    return Json(
                       new JsonActionResponse
                       {
                           Status = "Success",
                           Message = responseString
                       });
                }
                catch (Exception ext)
                {
                    SaveMessage("+" + phoneNumber, message, ext.Message);
                    return Json(
                       new JsonActionResponse
                       {
                           Status = "Error",
                           Message = ext.Message
                       });
                }
            }
            else
            {

                return Json(
                       new JsonActionResponse
                       {
                           Status = "Error",
                           Message = "Message not sent!"
                       });
            }
                        
        }

        private void SaveMessage(string sender, string message, string responseString)
        {
            SentSms sentSms = new SentSms { PhoneNumber = sender, Message = message, Response = responseString, SentDate = DateTime.UtcNow };
            SaveOrUpdateCommand.Execute(sentSms);
        }

        private string GeneratePostData(string message, string phoneNumber)
        {
            String postMessage = HttpUtility.UrlEncode(message);
            String strPost = "?phonenumber=%2B" + phoneNumber + "&user=" + AppSettings.SmsGatewayUserName + "&password=" + AppSettings.SmsGatewayPassword + "&text=" + postMessage;
            return strPost;
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
