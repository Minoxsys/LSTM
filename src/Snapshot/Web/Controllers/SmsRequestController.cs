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
using Web.Bootstrap;

namespace Web.Controllers
{
    public class SmsRequestController : Controller
    {
        public IManageReceivedSmsService ManageReceivedSmsService { get; set; }
        public ISmsRequestService SmsRequestService { get; set; }
        public IEmailMessageService EmailMessageService { get; set; }

        public ISaveOrUpdateCommand<RawSmsReceived> SaveCommandRawSmsReceived { get; set; }
        public ISaveOrUpdateCommand<MessageFromDrugShop> SaveCommandMessageFromDrugShop { get; set; }
        public ISaveOrUpdateCommand<MessageFromDispensary> SaveCommandMessageFromDispensary { get; set; }
        public ISaveOrUpdateCommand<WrongMessage> SaveCommandWrongMessage { get; set; }
        public IQueryService<MessageFromDispensary> QueryMessageFromDispensary { get; set; }
        public IQueryService<WrongMessage> QueryWrongMessage { get; set; }
        public IDeleteCommand<MessageFromDispensary> DeleteCommand { get; set; }

        private const string INVALIDNUMBERERRORMESSAGE = "Namba ya simu uliotumia haijasajiliwa na waongozi wa mtandao huu. Tafadhali wasiliana na utawala au tuma tena kwa kutumia namba ya simu iliyosajiliwa.Ahsante.";
        private const string INVALIDFORMATERRORMESSAGE = "Muundo wa ujumbe wako si sahihi.Tafadhali angalia na utume tena.Ahsante.";
        private const string ACTIVATESUCCESS = "Simu yako imewezeshwa.";
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
            Response.Status = "200 OK";
            Response.StatusCode = 200;

            if (ManageReceivedSmsService.DoesMessageStartWithKeyword(message) == false)
            {
                Response.Write("Wrong keyword.");
                return new EmptyResult();
            }

            RawSmsReceived rawSmsReceived = new RawSmsReceived { Content = message, Sender = msisdn, ReceivedDate = DateTime.UtcNow };
            rawSmsReceived = ManageReceivedSmsService.AssignOutpostToRawSmsReceivedBySenderNumber(rawSmsReceived);

            if (IsARegisteredPhoneNumber(rawSmsReceived) == false)
            {
                string responseMessage = ProcessMessageFromInvalidPhoneNumber(rawSmsReceived);
                Response.Write(responseMessage);
                return new EmptyResult();
            }

            if (ManageReceivedSmsService.IsMessageForActivation(rawSmsReceived))
            {
                string response = ActivatePhoneNumber(rawSmsReceived); 
                Response.Write(response);
                return new EmptyResult();
            }

            if (IsMessageFromDrugShop(rawSmsReceived))
            {
                string responseMessage = ProcessMessageFromDrugShop(rawSmsReceived);
                Response.Write(responseMessage);
                return new EmptyResult();
            }
            else
            {
                string responseMessage = ProcessMessageFromDispensary(rawSmsReceived);
                Response.Write(responseMessage);
                return new EmptyResult();
            }
        }

        private string ActivatePhoneNumber(RawSmsReceived rawSmsReceived)
        {
            ManageReceivedSmsService.ActivateThePhoneNumber(rawSmsReceived);
            SaveRawSmsReceived(rawSmsReceived, "", true);
            SmsRequestService.SendMessage(ACTIVATESUCCESS, rawSmsReceived);
            return ACTIVATESUCCESS;
        }

        private string  ProcessMessageFromInvalidPhoneNumber(RawSmsReceived rawSmsReceived)
        {
            SaveRawSmsReceived(rawSmsReceived, "Phone number is not valid.", false);
            SmsRequestService.SendMessage(INVALIDNUMBERERRORMESSAGE, rawSmsReceived);
            return INVALIDNUMBERERRORMESSAGE;
        }

        private void SaveRawSmsReceived(RawSmsReceived rawSmsReceived, string errorMessage, bool succeeded)
        {
            rawSmsReceived.ParseErrorMessage = errorMessage;
            rawSmsReceived.ParseSucceeded = succeeded;
            SaveCommandRawSmsReceived.Execute(rawSmsReceived);
        }

        private string ProcessMessageFromDrugShop(RawSmsReceived rawSmsReceived)
        {
            string responseMessage = "";

            rawSmsReceived = ManageReceivedSmsService.ParseRawSmsReceivedFromDrugShop(rawSmsReceived);
            SaveCommandRawSmsReceived.Execute(rawSmsReceived);
            
            if (rawSmsReceived.ParseSucceeded)
            {
                MessageFromDrugShop drugshopMessage = SaveDrugShopMessage(rawSmsReceived);

                if (ManageReceivedSmsService.DoesMessageContainRRCode(drugshopMessage) == false)
                {
                    string password = GeneratePassword();
                    responseMessage = SendMessageToDrugShopWithPassword(password, rawSmsReceived);
                    SendMessageToDispensary(password, rawSmsReceived, drugshopMessage);
                } 
            }
            else
            {
                responseMessage = INVALIDFORMATERRORMESSAGE;
                SmsRequestService.SendMessage(INVALIDFORMATERRORMESSAGE, rawSmsReceived);
                int noOfWrongMessages = SaveWrongMessage(rawSmsReceived);
                if (noOfWrongMessages % 3 == 0)
                    EmailMessageService.SendEmail(rawSmsReceived);

            }

            return responseMessage;
        }

        private MessageFromDrugShop SaveDrugShopMessage(RawSmsReceived rawSmsReceived)
        {
            MessageFromDrugShop drugshopMessage = ManageReceivedSmsService.CreateMessageFromDrugShop(rawSmsReceived);
            SaveCommandMessageFromDrugShop.Execute(drugshopMessage);
            return drugshopMessage;
        }

        private void SendMessageToDispensary(string password, RawSmsReceived rawSmsReceived, MessageFromDrugShop drugshopMessage)
        {
            string messageForDispensary = password + " " + ManageReceivedSmsService.CreateMessageToBeSentToDispensary(drugshopMessage);
            SmsRequestService.SendMessageToDispensary(messageForDispensary, rawSmsReceived);
        }

        private string SendMessageToDrugShopWithPassword(string password, RawSmsReceived rawSmsReceived)
        {
            string messageForDrugShop = password + " for " + rawSmsReceived.Content;
            SmsRequestService.SendMessage(messageForDrugShop, rawSmsReceived);
            return messageForDrugShop;
        }

        private string ProcessMessageFromDispensary(RawSmsReceived rawSmsReceived)
        {
            string responseMessage = "";

            rawSmsReceived = ManageReceivedSmsService.ParseRawSmsReceivedFromDispensary(rawSmsReceived);
            SaveCommandRawSmsReceived.Execute(rawSmsReceived);

            if (rawSmsReceived.ParseSucceeded)
            {
                string code = SaveMessageFromDispensary(rawSmsReceived);
                string confirmation = "Asante kwa ujumbe wako.Matibabu ya " + code + " yamewekwa kwenye kumbukumbu.IntHEC (Thank you for your message.The message with code " + code + " was saved.IntHEC)";
                SmsRequestService.SendMessage(confirmation, rawSmsReceived);
                responseMessage = confirmation;
            }
            else
            {
                responseMessage = INVALIDFORMATERRORMESSAGE;
                SmsRequestService.SendMessage(INVALIDFORMATERRORMESSAGE, rawSmsReceived);
                int noOfWrongMessages = SaveWrongMessage(rawSmsReceived);
                if (noOfWrongMessages % 3 == 0)
                    EmailMessageService.SendEmail(rawSmsReceived);
            }

            return responseMessage;
        }

        private bool IsMessageFromDrugShop(RawSmsReceived rawSmsReceived)
        {
            return rawSmsReceived.OutpostType == 0;
        }

        private bool IsARegisteredPhoneNumber(RawSmsReceived rawSmsReceived)
        {
            return rawSmsReceived.OutpostId != Guid.Empty;
        }

        private int SaveWrongMessage(RawSmsReceived rawSmsReceived)
        {
            var message = QueryWrongMessage.Query().Where(it => it.PhoneNumber == rawSmsReceived.Sender).FirstOrDefault();
            if (message != null)
            {
                message.NoOfWrongMessages++;
                SaveCommandWrongMessage.Execute(message);
                return message.NoOfWrongMessages;
            }

            WrongMessage newMessage = new WrongMessage { PhoneNumber = rawSmsReceived.Sender, NoOfWrongMessages = 1};
            SaveCommandWrongMessage.Execute(newMessage);
            return 1;
            
        }

        private string SaveMessageFromDispensary(RawSmsReceived rawSmsReceived)
        {
            MessageFromDispensary dispensaryMessage = ManageReceivedSmsService.CreateMessageFromDispensary(rawSmsReceived);
            var existingMessage = QueryMessageFromDispensary.Query().Where(it => it.MessageFromDrugShop.Id == dispensaryMessage.MessageFromDrugShop.Id).OrderByDescending(it => it.SentDate).FirstOrDefault();
            if (existingMessage != null)
                DeleteCommand.Execute(existingMessage);

            SaveCommandMessageFromDispensary.Execute(dispensaryMessage);
            return dispensaryMessage.MessageFromDrugShop.IDCode;
        }

        private string GeneratePassword()
        {
            Random random = new Random();
            int randomNumber = random.Next(0, 10);
            return passwordList[randomNumber];
        }
    }
}
