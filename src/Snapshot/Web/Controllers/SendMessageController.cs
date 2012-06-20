using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.Shared;
using Web.Services;
using Web.Bootstrap;

namespace Web.Controllers
{
    public class SendMessageController : Controller
    {
        public ISmsGatewayService smsGatewayService { get; set; }

        public ActionResult Overview()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Send(string message)
        {
            string request = CreatePostData(message);

            try
            {
                smsGatewayService.SendSmsRequest(request);

                return Json(
                   new JsonActionResponse
                   {
                       Status = "Success",
                       Message = String.Format("Message sent")
                    });
            }
            catch (Exception)
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = String.Format("Message NOT sent")
                    });
            }
                        
        }

        private string CreatePostData(string model)
        {
            string response = "<?xml version='1.0' encoding='UTF-8'?><sms-response login='" + AppSettings.SmsGatewayUserName + "' password='" + AppSettings.SmsGatewayPassword + "' delivery-notification-requested='true' version='1.0'> ";
            response += "<message id='1' msisdn='0040747651059' service-number='" + AppSettings.SmsGatewayShortcode + "' ";
            response += "validity-period='3' priority='1'>";
            response += "<content type='text/plain'>" + model + "</content></message></sms-response>";

            return response;
        }

    }
}
