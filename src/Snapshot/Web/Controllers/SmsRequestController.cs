using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.SmsRequest;
using Core.Persistence;
using Domain;
using Web.Services;
using Core.Domain;
using System.Globalization;
using Persistence.Queries.Outposts;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Text;

namespace Web.Controllers
{
    public class SmsRequestController : Controller
    {
        public IManageReceivedSmsService ManageReceivedSmsService { get; set; }
        public ISmsRequestService SmsRequestService { get; set; }

        public ISaveOrUpdateCommand<RawSmsReceived> SaveCommandRawSmsReceived { get; set; }
        public ISaveOrUpdateCommand<MessageFromDrugShop> SaveCommandMessageFromDrugShop { get; set; }
        public ISaveOrUpdateCommand<MessageFromDispensary> SaveCommandMessageFromDispensary { get; set; }

        private const string INVALIDNUMBERERRORMESSAGE = "Your phone is not registered. Please contact your administrator or resend from a registered phone. Thanks.";
        private const string INVALIDFORMATERRORMESSAGE = "The format of your message is incorrect. Please check and retry. Thank you.";
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";
        private IFormatProvider FormatProvider = CultureInfo.InvariantCulture;

        public ActionResult Create()
        {
            SmsRequestCreateModel model = new SmsRequestCreateModel();
            return View(model);
        }

        public ActionResult Overview()
        {
            var model = new SmsRequestCreateModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ReceiveSms()
        {
           
            var stream = Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = Encoding.UTF8.GetString(buffer);


            RawSmsReceived test2 = new RawSmsReceived();
            test2.SmsId = "1";
            test2.Sender = "After";
            test2.ServiceNumber = "152";
            test2.Content = "body";
            test2.Operator = Request.ContentType;
            test2.Keyword = "After";
            test2.OperatorId = xml;
            test2.ReceivedDate = DateTime.UtcNow;
            SaveCommandRawSmsReceived.Execute(test2);


            RawSmsReceived rawSmsReceived = ManageReceivedSmsService.GetRawSmsReceivedFromXMLString(xml);
            if (rawSmsReceived == null)
            {
                SmsRequestService.SendResponseMessage(rawSmsReceived);
                return new EmptyResult();
            }
            rawSmsReceived = ManageReceivedSmsService.AssignOutpostToRawSmsReceivedBySenderNumber(rawSmsReceived);

            if (rawSmsReceived.OutpostId == Guid.Empty)
            {
                rawSmsReceived.ParseErrorMessage = "Phone number is not valid.";
                rawSmsReceived.ParseSucceeded = false;
                SaveCommandRawSmsReceived.Execute(rawSmsReceived);

                SmsRequestService.SendMessage(INVALIDNUMBERERRORMESSAGE, rawSmsReceived);
                return new EmptyResult();
            }
            SaveCommandRawSmsReceived.Execute(rawSmsReceived);

            if (rawSmsReceived.OutpostType == 0)
            {
                rawSmsReceived = ManageReceivedSmsService.ParseRawSmsReceivedFromDrugShop(rawSmsReceived);
                SaveCommandRawSmsReceived.Execute(rawSmsReceived);
                if (rawSmsReceived.ParseSucceeded)
                {
                    MessageFromDrugShop message = ManageReceivedSmsService.CreateMessageFromDrugShop(rawSmsReceived);
                    SaveCommandMessageFromDrugShop.Execute(message);

                    SmsRequestService.SendResponseMessage(rawSmsReceived);
                    SmsRequestService.SendMessageToDispensary(message, rawSmsReceived);
                }
                else
                {
                    SmsRequestService.SendMessage(rawSmsReceived.ParseErrorMessage, rawSmsReceived);
                }
            }
            else
            {
                rawSmsReceived = ManageReceivedSmsService.ParseRawSmsReceivedFromDispensary(rawSmsReceived);
                SaveCommandRawSmsReceived.Execute(rawSmsReceived);
                if (rawSmsReceived.ParseSucceeded)
                {
                    MessageFromDispensary message = ManageReceivedSmsService.CreateMessageFromDispensary(rawSmsReceived);
                    SaveCommandMessageFromDispensary.Execute(message);
                }
                else
                {
                    SmsRequestService.SendMessage(INVALIDFORMATERRORMESSAGE, rawSmsReceived);
                }
            }


            return new EmptyResult();
        }
    }
}
