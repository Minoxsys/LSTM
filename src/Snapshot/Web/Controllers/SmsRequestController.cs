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

        private const string INVALIDNUMBERERRORMESSAGE = "Namba ya simu uliotumia haijasajiliwa na waongozi wa mtandao huu. Tafadhali wasiliana na utawala au tuma tena kwa kutumia namba ya simu iliyosajiliwa.Ahsante.";
        private const string INVALIDFORMATERRORMESSAGE = "Muundo wa ujumbe wako si sahihi.Tafadhali angalia na utume tena.Ahsante.";
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
        public void ReceiveSms(string message, string msisdn)
        {
            RawSmsReceived rawSmsReceived = new RawSmsReceived { Content = message, Sender = msisdn, ReceivedDate = DateTime.UtcNow};
            rawSmsReceived = ManageReceivedSmsService.AssignOutpostToRawSmsReceivedBySenderNumber(rawSmsReceived);

            if (rawSmsReceived.OutpostId == Guid.Empty)
            {
                rawSmsReceived.ParseErrorMessage = "Phone number is not valid.";
                rawSmsReceived.ParseSucceeded = false;
                SaveCommandRawSmsReceived.Execute(rawSmsReceived);

                SmsRequestService.SendMessage(INVALIDNUMBERERRORMESSAGE, rawSmsReceived);
            }
            else
            {
                SaveCommandRawSmsReceived.Execute(rawSmsReceived);

                if (rawSmsReceived.OutpostType == 0)
                {
                    rawSmsReceived = ManageReceivedSmsService.ParseRawSmsReceivedFromDrugShop(rawSmsReceived);
                    SaveCommandRawSmsReceived.Execute(rawSmsReceived);
                    if (rawSmsReceived.ParseSucceeded)
                    {
                        MessageFromDrugShop drugshopMessage = ManageReceivedSmsService.CreateMessageFromDrugShop(rawSmsReceived);
                        SaveCommandMessageFromDrugShop.Execute(drugshopMessage);

                        SmsRequestService.SendMessageToDispensary(drugshopMessage, rawSmsReceived);
                    }
                    else
                    {
                        SmsRequestService.SendMessage(INVALIDFORMATERRORMESSAGE, rawSmsReceived);
                    }
                }
                else
                {
                    rawSmsReceived = ManageReceivedSmsService.ParseRawSmsReceivedFromDispensary(rawSmsReceived);
                    SaveCommandRawSmsReceived.Execute(rawSmsReceived);
                    if (rawSmsReceived.ParseSucceeded)
                    {
                        MessageFromDispensary dispensaryMessage = ManageReceivedSmsService.CreateMessageFromDispensary(rawSmsReceived);
                        SaveCommandMessageFromDispensary.Execute(dispensaryMessage);
                    }
                    else
                    {
                        SmsRequestService.SendMessage(INVALIDFORMATERRORMESSAGE, rawSmsReceived);
                    }
                }
            }

        }
    }
}
