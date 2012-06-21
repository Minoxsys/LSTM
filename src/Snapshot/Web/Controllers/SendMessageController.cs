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
            bulkRequest += "<bulk-request login='"+AppSettings.SmsGatewayUserName+"' password='"+AppSettings.SmsGatewayPassword+"' request-id='1001' delivery-notification-requested='true' version='1.0'>";
            bulkRequest += "<message id='1' msisdn='255787959070' service-number='" + AppSettings.SmsGatewayShortcode + "' validity-period='3' priority='1'> ";
            bulkRequest += "<content type='text/plain'>Test can you please send a message to 15046. message: Test SR120365F S1 Thanks</content>";
            bulkRequest += "</message></bulk-request>";

            return bulkRequest;
        }
    }
}
