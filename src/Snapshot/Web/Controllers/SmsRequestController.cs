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
        private string[] passwordList = new string[] { "Simba", "Tembo", "Twiga", "Chui", "Nyati", "Duma", "Fisi", "Kiboko", "Kifaru", "Sungura", "Swala" };
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

        [HttpGet]
        public ActionResult ReceiveSms(string message, string msisdn)
        {
            RawSmsReceived rawSmsReceived = new RawSmsReceived { Content = message, Sender = msisdn, ReceivedDate = DateTime.UtcNow };
            rawSmsReceived = ManageReceivedSmsService.AssignOutpostToRawSmsReceivedBySenderNumber(rawSmsReceived);

            if (rawSmsReceived.OutpostId == Guid.Empty)
            {
                SaveRawSmsReceived(rawSmsReceived, "Phone number is not valid.", false);
                SmsRequestService.SendMessage(INVALIDNUMBERERRORMESSAGE, rawSmsReceived);
            }
            else
            {
                if (rawSmsReceived.OutpostType == 0)
                {
                    rawSmsReceived = ManageReceivedSmsService.ParseRawSmsReceivedFromDrugShop(rawSmsReceived);
                    SaveCommandRawSmsReceived.Execute(rawSmsReceived);

                    if (rawSmsReceived.ParseSucceeded)
                    {
                        MessageFromDrugShop drugshopMessage = ManageReceivedSmsService.CreateMessageFromDrugShop(rawSmsReceived);
                        SaveCommandMessageFromDrugShop.Execute(drugshopMessage);

                        string password = GeneratePassword();
                        string messageForDrugShop = password + " for " + rawSmsReceived.Content;
                        string messageForDispensary = password + " " + ManageReceivedSmsService.CreateMessageToBeSentToDispensary(drugshopMessage);

                        SmsRequestService.SendMessage(messageForDrugShop, rawSmsReceived);
                        SmsRequestService.SendMessageToDispensary(messageForDispensary, rawSmsReceived);
                    }
                    else
                        SmsRequestService.SendMessage(INVALIDFORMATERRORMESSAGE, rawSmsReceived);
                }
                else
                {
                    rawSmsReceived = ManageReceivedSmsService.ParseRawSmsReceivedFromDispensary(rawSmsReceived);
                    SaveCommandRawSmsReceived.Execute(rawSmsReceived);

                    if (rawSmsReceived.ParseSucceeded)
                        SaveMessageFromDispensary(rawSmsReceived);
                    else
                        SmsRequestService.SendMessage(INVALIDFORMATERRORMESSAGE, rawSmsReceived);
                }
            }

            return new EmptyResult();
        }

        private void SaveRawSmsReceived(RawSmsReceived rawSmsReceived, string errorMessage, bool succeeded)
        {
            rawSmsReceived.ParseErrorMessage = errorMessage;
            rawSmsReceived.ParseSucceeded = succeeded;
            SaveCommandRawSmsReceived.Execute(rawSmsReceived);
        }

        private void SaveMessageFromDispensary(RawSmsReceived rawSmsReceived)
        {
            MessageFromDispensary dispensaryMessage = ManageReceivedSmsService.CreateMessageFromDispensary(rawSmsReceived);
            SaveCommandMessageFromDispensary.Execute(dispensaryMessage);
        }

        private string GeneratePassword()
        {
            Random random = new Random();
            int randomNumber = random.Next(0, 10);
            return passwordList[randomNumber];
        }
    }
}
