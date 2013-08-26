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
        public IQueryService<MessageFromDrugShop> QueryMessageFromDrugShop { get; set; }
        public IQueryService<WrongMessage> QueryWrongMessage { get; set; }
        public IDeleteCommand<MessageFromDispensary> DeleteCommand { get; set; }
        public IQueryOutposts queryOutposts { get; set; }

        private const string INVALIDNUMBERERRORMESSAGE = "Namba ya simu uliotumia haijasajiliwa na waongozi wa mtandao huu. Tafadhali wasiliana na utawala au tuma tena kwa kutumia namba ya simu iliyosajiliwa.Ahsante.";
        private const string INVALIDFORMATERRORMESSAGE1 = "Ujumbe wako sio sahihi, soma kwa makini kadi ya maelekezo, andika ujumbe sahihi, uhakiki ujumbe na utume tena.";
        private const string INVALIDFORMATERRORMESSAGE2 = "Umetuma jumbe mbili zilizokosewa, tafadhali sahihisha makosa uliyofanya na utume tena. Usitume ujumbe mpaka utakapokuwa na uhakika kwamba ujumbe uko sahihi.";
        private const string INVALIDFORMATERRORMESSAGE3 = "Umetuma jumbe tatu zilizokosewa, tafadhali acha kutuma na uwasiliane na mtawala kwa msaada zaidi.";
        private const string ACTIVATESUCCESS = "Simu yako imewezeshwa.";
        private string[] passwordList = new string[] { "Simba", "Tembo", "Twiga", "Chui", "Nyati", "Duma", "Mamba", "Kiboko", "Kifaru", "Sungura", "Swala" };
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
                int answer;
                if (ManageReceivedSmsService.IsAttendingReminderAnswer(message, msisdn, out answer))
                {
                    SaveAnswer(answer, msisdn);
                }
                else
                {
                    Response.Write("Wrong keyword.");
                    return new EmptyResult();
                }
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

        private void SaveAnswer(int answer, string sender)
        {
            var drugShopMessage = QueryMessageFromDrugShop.Query()
                                                          .Where(
                                                              m =>
                                                              m.PatientPhoneNumber == sender && m.PatientReferralConsumed == false &&
                                                              m.PatientReferralReminderSentDate.HasValue)
                                                          .OrderByDescending(msg => msg.PatientReferralReminderSentDate)
                                                          .FirstOrDefault();
            //always target the most recent doctor reservation (for previous ones it is considered the patient did not want to answer)
            if (drugShopMessage != null)
            {
                drugShopMessage.ReminderAnswer = answer;
                SaveCommandMessageFromDrugShop.Execute(drugShopMessage);
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

                if (ManageReceivedSmsService.DoesMessageContainRRCode(drugshopMessage) == true)
                {
                    responseMessage = "Asante kwa ujumbe wako.IntHEC";
                    SmsRequestService.SendMessage(responseMessage, rawSmsReceived);
                }
                else
                {
                    string password = GeneratePassword();
                    responseMessage = SendMessageToDrugShopWithPassword(password, rawSmsReceived);
                    if (!string.IsNullOrEmpty(drugshopMessage.PatientPhoneNumber))
                    {
                        Outpost warehouse = queryOutposts.GetWarehouse(rawSmsReceived.OutpostId);
                        SendMessageToPatientWithPassword(password, drugshopMessage.PatientPhoneNumber, warehouse != null ? warehouse.Name : string.Empty);
                    }
                    SendMessageToDispensary(password, rawSmsReceived, drugshopMessage);
                }
            }
            else
            {
                responseMessage = ProcessWrongMessage(rawSmsReceived);
            }

            return responseMessage;
        }

        private void SendMessageToPatientWithPassword(string password, string patientPhoneNumber, string dispensaryName)
        {
            string messageForDrugShop = string.Format(Resources.Resources.PatientConfirmationSmsText, password, dispensaryName);
            SmsRequestService.SendMessage(messageForDrugShop, patientPhoneNumber);
        }

        private string ProcessWrongMessage(RawSmsReceived rawSmsReceived)
        {
            string responseMessage = string.Empty;
            int noOfWrongMessages = SaveWrongMessage(rawSmsReceived);

            if (noOfWrongMessages % 3 == 1)
            {
                responseMessage = IsInvalidPatientPhoneNumber(rawSmsReceived)
                                      ? Resources.Resources.InvalidPatientTelphoneDescriptiveMessage
                                      : INVALIDFORMATERRORMESSAGE1;
            }

            if (noOfWrongMessages % 3 == 2)
            {
                responseMessage = IsInvalidPatientPhoneNumber(rawSmsReceived)
                                      ? Resources.Resources.InvalidPatientTelphoneDescriptiveMessage
                                      : INVALIDFORMATERRORMESSAGE2;
            }
            if (noOfWrongMessages % 3 == 0)
            {
                responseMessage = INVALIDFORMATERRORMESSAGE3;
                EmailMessageService.SendEmail(rawSmsReceived);
            }

            SmsRequestService.SendMessage(responseMessage, rawSmsReceived);

            return responseMessage;
        }

        private bool IsInvalidPatientPhoneNumber(RawSmsReceived rawSmsReceived)
        {
            return String.Compare(rawSmsReceived.ParseErrorMessage, Resources.Resources.InvalidPatientPhoneNumber, StringComparison.OrdinalIgnoreCase) == 0;
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
                responseMessage = ProcessWrongMessage(rawSmsReceived);
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
            var message = QueryWrongMessage.Query().Where(it => it.PhoneNumber == rawSmsReceived.Sender && it.SentDate.Date == DateTime.UtcNow.Date).FirstOrDefault();
            if (message != null)
            {
                message.NoOfWrongMessages++;
                SaveCommandWrongMessage.Execute(message);
                return message.NoOfWrongMessages;
            }

            WrongMessage newMessage = new WrongMessage { PhoneNumber = rawSmsReceived.Sender, NoOfWrongMessages = 1, SentDate = DateTime.UtcNow};
            SaveCommandWrongMessage.Execute(newMessage);
            return 1;
            
        }

        private string SaveMessageFromDispensary(RawSmsReceived rawSmsReceived)
        {
            MessageFromDispensary dispensaryMessage = ManageReceivedSmsService.CreateMessageFromDispensary(rawSmsReceived);
            var existingMessage = QueryMessageFromDispensary.Query().Where(it => it.MessageFromDrugShop.Id == dispensaryMessage.MessageFromDrugShop.Id).OrderByDescending(it => it.SentDate).FirstOrDefault();
            if (existingMessage != null)
                DeleteCommand.Execute(existingMessage);

            dispensaryMessage.MessageFromDrugShop.PatientReferralConsumed = true;
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
