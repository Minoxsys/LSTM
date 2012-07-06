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

namespace Web.Controllers
{
    public class SendMessageController : Controller
    {
        public ISmsGatewayService smsGatewayService { get; set; }
        public IEmailService EmailService { get; set; }

        public ActionResult Overview()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Send(string message)
        {
            string request = CreatePostData(message);

            string responseMessage = "";
            try
            {
                
                responseMessage = smsGatewayService.SendSmsRequest(request);
                
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Success",
                       Message = String.Format("Message sent: " + responseMessage)
                    });
            }
            catch (Exception)
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = String.Format("Message NOT sent: " + responseMessage)
                    });
            }
                        
        }

        private string CreatePostData(string model)
        {
            //string response = "<?xml version='1.0' encoding='UTF-8'?><sms-response login='" + AppSettings.SmsGatewayUserName + "' password='" + AppSettings.SmsGatewayPassword + "' delivery-notification-requested='true' version='1.0'> ";
            //response += "<message id='1' msisdn='0040747651059' service-number='" + AppSettings.SmsGatewayShortcode + "' ";
            //response += "validity-period='3' priority='1'>";
            //response += "<content type='text/plain'>" + model + "</content></message></sms-response>";

            string bulkRequest = "<?xml version='1.0' encoding='UTF-8'?>";
            bulkRequest += "<bulk-request login='"+AppSettings.SmsGatewayUserName+"' password='"+AppSettings.SmsGatewayPassword+"' request-id='1013' delivery-notification-requested='true' version='1.0'>";
            bulkRequest += "<message id='1' msisdn='255787959070' service-number='" + AppSettings.SmsGatewayShortcode + "' validity-period='3' priority='1'> ";
            bulkRequest += "<content type='text/plain'>Test from Claudia</content>";
            bulkRequest += "</message></bulk-request>";

            return bulkRequest;
        }

        [HttpPost]
        public JsonResult SendXml(string message)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><sms-request version=\"1.0\"><message id=\"6644999\" keyword=\"test\" message-count=\"1\" msisdn=\"0040747651059\" operator=\"tigo-smpp\" operator-id=\"10003\" service-number=\"15046\" submit-date=\"2012-06-28 14:01:49\"><content type=\"text/plain\">test DR120387F S1</content></message></sms-request>";
            string url = "http://lstm-staging.apphb.com/SmsRequest/ReceiveSms";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(xml);
            req.Method = "POST";
            req.ContentType = "text/xml;charset=utf-8";
            req.ContentLength = requestBytes.Length;
            Stream requestStream = req.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();


            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.Default);
            string backstr = sr.ReadToEnd();
            string description = res.StatusDescription;

            sr.Close();
            res.Close();

            return Json(
                   new JsonActionResponse
                   {
                       Status = backstr,
                       Message = String.Format(backstr)
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

        [HttpPost]
        public JsonResult DBBackup(string message)
        {
            //bool bBackUpStatus = true;

            //Cursor.Current = Cursors.WaitCursor;

            //if (Directory.Exists(@"c:\SQLBackup"))
            //{
            //    if (File.Exists(@"c:\SQLBackup\wcBackUp1.bak"))
            //    {
            //        if (MessageBox.Show(@"Do you want to replace it?", "Back", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //        {
            //            File.Delete(@"c:\SQLBackup\wcBackUp1.bak");
            //        }
            //        else
            //            bBackUpStatus = false;
            //    }
            //}
            //else
            //    Directory.CreateDirectory(@"c:\SQLBackup");

            //if (bFileStatus)
            //{
                //Connect to DB
                SqlConnection connect;
                string con =  "Data Source=.\\sqlexpress;Initial Catalog=LSTMDB;Integrated Security=True";
                connect = new SqlConnection(con);
                connect.Open();

                //Execute SQL---------------
                SqlCommand command;
                command = new SqlCommand(@"backup database LSTMDB to disk ='D:\wcBackUp1.bak' with init,stats=10", connect);
                command.ExecuteNonQuery();
                //-------------------------------------------------------------------------------------------------------------------------------

                connect.Close();
                return Json(
                   new JsonActionResponse
                   {
                       Status = "OK",
                       Message = "The support of the database was successfully performed"
                   });
                
            //}

        }

        [HttpPost]
        public JsonResult RestoreDB(string message)
        {
            try
            {
                //if (File.Exists(@"c:\SQLBackup\wcBackUp1.bak"))
                //{
                //    if (MessageBox.Show("Are you sure you restore?", "Back", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                //    {
                        //Connect SQL-----------
                        SqlConnection connect;
                        string con = "Data Source=.\\sqlexpress;Initial Catalog=LSTMDB;Integrated Security=True";
                        connect = new SqlConnection(con);
                        connect.Open();
                        //-----------------------------------------------------------------------------------------

                        //Excute SQL----------------
                        SqlCommand command;
                        command = new SqlCommand("use master", connect);
                        command.ExecuteNonQuery();
                        command = new SqlCommand("drop database LSTMDB", connect);
                        command.ExecuteNonQuery();
                        command = new SqlCommand(@"restore database LSTMDB from disk = 'D:\wcBackUp1.bak'", connect);
                        command.ExecuteNonQuery();
                        //--------------------------------------------------------------------------------------------------------
                        connect.Close();
                        
                return Json(
                   new JsonActionResponse
                   {
                       Status = "OK",
                       Message = "Database has been restored"
                   });
                
            }
            catch (Exception exp)
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "OK",
                       Message = exp.Message
                   });
            }
        }
    }
}
