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

        public ActionResult ReceiveSms(string msisdn, string servicenumber, string operatorr, string date, string text)
        {
            RawSmsReceived rawSmsReceived = new RawSmsReceived();
            rawSmsReceived.Sender = msisdn;
            rawSmsReceived.Content = text;
            date = HttpUtility.UrlDecode(date);
            DateTime dateRetult;
            if (DateTime.TryParseExact(date, DateFormat, FormatProvider, DateTimeStyles.None, out dateRetult))
                rawSmsReceived.ReceivedDate = dateRetult;
            else
                rawSmsReceived.ReceivedDate = DateTime.UtcNow;

            rawSmsReceived = ManageReceivedSmsService.AssignOutpostToRawSmsReceivedBySenderNumber(rawSmsReceived);

            if (rawSmsReceived.OutpostId == Guid.Empty)
            {
                rawSmsReceived.ParseErrorMessage = "Phone number is not valid.";
                rawSmsReceived.ParseSucceeded = false;
                SaveCommandRawSmsReceived.Execute(rawSmsReceived);

                SmsRequestService.SendMessage(INVALIDNUMBERERRORMESSAGE, servicenumber);
                return null;
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

                    SmsRequestService.SendMessage(INVALIDNUMBERERRORMESSAGE, servicenumber);

                }
                else
                {
                    SmsRequestService.SendMessage(rawSmsReceived.ParseErrorMessage, servicenumber);
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
                    bool IsSent = false;
                    do
                        IsSent = SmsRequestService.SendMessage(INVALIDFORMATERRORMESSAGE, servicenumber);
                    while (IsSent);
                }
            }

            return null;
        }
    }
}
