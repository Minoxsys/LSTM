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
        public JsonResult Send(string message)
        {
            string url = @"http://lstm-staging.apphb.com/SmsRequest/ReceiveSms?message='lalalalalalla'&msisdn='hhhh'";
            string responseMessage = "lalalalalalalala";
            try
            {

                responseMessage = HttpService.Post(url, responseMessage);
                
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


        [HttpPost]
        public JsonResult SendXml(string message)
        {

            return Json(
                   new JsonActionResponse
                   {
                       Status = "",
                       Message = ""
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
            string finalDirectory = FileService.GetDBBackupDirector();

            try{

                SqlConnection connect;
                string con = AppSettings.ServerConnectionStrings;  //"Data Source=.\\sqlexpress;Initial Catalog=LSTMDB;Integrated Security=True";
                connect = new SqlConnection(con);
                connect.Open();

                SqlCommand command;
                string file = finalDirectory + "\\Backup_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + ".bak";
                command = new SqlCommand(@"backup database " + AppSettings.ServerDatabase + " to disk ='" + file + "' with init,stats=10", connect);
                command.ExecuteNonQuery();

                connect.Close();

                return Json(
                       new JsonActionResponse
                       {
                           Status = "Success",
                           Message = "Backup complete"
                       });
            }
            catch (Exception exp)
            {
                return Json(
                       new JsonActionResponse
                       {
                           Status = "Error",
                           Message = exp.Message
                       });
            }

        }

        [HttpPost]
        public JsonResult RestoreDB(string fileName)
        {
            string backupDirectory = FileService.GetDBBackupDirector();
            string file = backupDirectory + "\\" + fileName;

            try
            {
                if (FileService.ExistsFile(file))
                {
                    SqlConnection connect;
                    string con = AppSettings.ServerConnectionStrings;
                    connect = new SqlConnection(con);
                    connect.Open();

                    SqlCommand command;
                    command = new SqlCommand("use master", connect);
                    command.ExecuteNonQuery();
                    command = new SqlCommand("drop database "+ AppSettings.ServerDatabase+" ", connect);
                    command.ExecuteNonQuery();
                    command = new SqlCommand(@"restore database " + AppSettings.ServerDatabase + " from disk = '" + file + "'", connect);
                    command.ExecuteNonQuery();
                    connect.Close();

                    return Json(
                       new JsonActionResponse
                       {
                           Status = "Success",
                           Message = "Database has been restored"
                       });
                }
                else
                {
                    return Json(
                       new JsonActionResponse
                       {
                           Status = "Error",
                           Message = "Filename " + fileName + " does not exist!"
                       });
                }
                
            }
            catch (Exception exp)
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = exp.Message
                   });
            }
        }

        [HttpPost]
        public JsonResult DeleteFile(string fileName)
        {
            string backupDirectory = FileService.GetDBBackupDirector();
            string file = backupDirectory + "\\" + fileName;
            
            try
            {
                if (FileService.ExistsFile(file))
                {
                    FileService.DeleteFile(file);
                    return Json(
                        new JsonActionResponse
                        {
                            Status = "Success",
                            Message = "File " + fileName + " has been deleted"
                        });
                }
                else
                {
                    return Json(
                        new JsonActionResponse
                        {
                            Status = "Error",
                            Message = "File " + fileName + " does not exist!"
                        });
                }
            }
            catch (Exception exp)
            {
                return Json(
                    new JsonActionResponse
                    {
                        Status = "Error",
                        Message = exp.Message
                    });
            }
           
        }
        

        [HttpGet]
        public JsonResult GetBackupFileList()
        {
            string backupDirector = FileService.GetDBBackupDirector() + "\\DBBackup";
            if (FileService.ExistsDirectory(backupDirector)){

                string[] fileList = FileService.GetFilesFromDirectory(backupDirector);

                List<FileBackupModel> files = new List<FileBackupModel>();
                foreach (var file in fileList)
                {
                    string fileName = file.Substring(backupDirector.Length + 1, file.Length - backupDirector.Length-1);
                    var fi = new FileInfo(file);
                    long size = fi.Length;
                    FileBackupModel model = new FileBackupModel { Name = fileName, Size = size.ToString() };
                    files.Add(model);
                }
                files = files.OrderByDescending(it => it.Name).ToList();

                return Json(new FileBackupIndexOutputModel
                {
                    Files = files.ToArray(),
                    TotalItems = files.Count()
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new FileBackupIndexOutputModel
            {
                Files = null,
                TotalItems = 0
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
