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
        private string[] passwordList = new string[] { "Simba", "Tembo", "Twiga", "Chui", "Nyati", "Duma", "Fisi", "Kiboko", "Kifaru", "Sungura", "Swala" };
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";
        private string KEYWORD = "AFYA";
        private string REFUSEDCODE = "RR";

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
            string responseMessage = "";

            RawSmsReceived rawSmsReceived = new RawSmsReceived { Content = message, Sender = msisdn, ReceivedDate = DateTime.UtcNow };
            rawSmsReceived = ManageReceivedSmsService.AssignOutpostToRawSmsReceivedBySenderNumber(rawSmsReceived);

            if (message.Substring(0, 4).ToUpper() == KEYWORD)
            {

                if (rawSmsReceived.OutpostId == Guid.Empty)
                {
                    SaveRawSmsReceived(rawSmsReceived, "Phone number is not valid.", false);
                    responseMessage = INVALIDNUMBERERRORMESSAGE;
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
                            if (drugshopMessage.ServicesNeeded.FirstOrDefault(it => it.Code == REFUSEDCODE) == null)
                            {
                                string password = GeneratePassword();
                                string messageForDrugShop = password + " for " + rawSmsReceived.Content;
                                string messageForDispensary = password + " " + ManageReceivedSmsService.CreateMessageToBeSentToDispensary(drugshopMessage);

                                responseMessage = messageForDrugShop;
                                SmsRequestService.SendMessage(messageForDrugShop, rawSmsReceived);
                                SmsRequestService.SendMessageToDispensary(messageForDispensary, rawSmsReceived);
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
                    }
                    else
                    {
                        rawSmsReceived = ManageReceivedSmsService.ParseRawSmsReceivedFromDispensary(rawSmsReceived);
                        SaveCommandRawSmsReceived.Execute(rawSmsReceived);

                        if (rawSmsReceived.ParseSucceeded)
                            SaveMessageFromDispensary(rawSmsReceived);
                        else
                        {
                            responseMessage = INVALIDFORMATERRORMESSAGE;
                            SmsRequestService.SendMessage(INVALIDFORMATERRORMESSAGE, rawSmsReceived);
                            int noOfWrongMessages = SaveWrongMessage(rawSmsReceived);
                            if (noOfWrongMessages % 3 == 0)
                                EmailMessageService.SendEmail(rawSmsReceived);
                        }
                    }
                }
            }
            Response.Status = "200 OK";
            Response.StatusCode = 200;
            Response.Write(responseMessage);

            return new EmptyResult();
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

        private void SaveRawSmsReceived(RawSmsReceived rawSmsReceived, string errorMessage, bool succeeded)
        {
            rawSmsReceived.ParseErrorMessage = errorMessage;
            rawSmsReceived.ParseSucceeded = succeeded;
            SaveCommandRawSmsReceived.Execute(rawSmsReceived);
        }

        private void SaveMessageFromDispensary(RawSmsReceived rawSmsReceived)
        {
            MessageFromDispensary dispensaryMessage = ManageReceivedSmsService.CreateMessageFromDispensary(rawSmsReceived);
            var existingMessage = QueryMessageFromDispensary.Query().Where(it => it.MessageFromDrugShop.Id == dispensaryMessage.MessageFromDrugShop.Id).OrderByDescending(it => it.SentDate).FirstOrDefault();
            if (existingMessage != null)
                DeleteCommand.Execute(existingMessage);

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
