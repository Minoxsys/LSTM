using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using Persistence.Queries.Outposts;
using Core.Persistence;
using Web.Models.SmsRequest;
using Web.Bootstrap;

namespace Web.Services
{
    public class SmsRequestService : ISmsRequestService
    {
        private ISmsGatewayService smsGatewayService;
        private IQueryOutposts queryOutposts;
        private IQueryService<Contact> queryContact;

        private const string DateFormat = "ddMMyy";

        public SmsRequestService(ISmsGatewayService smsGatewayService, IQueryOutposts queryOutposts, IQueryService<Contact> queryContact)
        {
            this.smsGatewayService = smsGatewayService;
            this.queryOutposts = queryOutposts;
            this.queryContact = queryContact;
        }

        public bool SendMessage(string message,  RawSmsReceived response)
        {
            ResponseModel model = new ResponseModel();
            model.Id = "1";
            model.Content = message;
            model.DeferDate = "";
            model.Operator = "";
            model.PhoneNumber = response.Sender;
            model.Priority = "1";
            model.RefId = response.SmsId;
            model.ServiceNo = response.ServiceNumber;
            model.Valability = "3";

            string request = CreatePostData(model);

            try
            {
                smsGatewayService.SendSmsRequest(request);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool SendMessageToDispensary(MessageFromDrugShop message, RawSmsReceived rawSms)
        {
            ResponseModel model = new ResponseModel();
            model.Id = "1";
            model.Content = CreateMessageToBeSentToDispensary(message);
            model.DeferDate = "";
            model.Operator = "";
            model.PhoneNumber = GetWarehousePhoneNumber(message.OutpostId);
            model.Priority = "1";
            model.RefId = rawSms.SmsId;
            model.ServiceNo = rawSms.ServiceNumber;
            model.Valability = "3";

            string request = CreatePostData(model);

            if (model.PhoneNumber != null)
            {
                try
                {
                    smsGatewayService.SendSmsRequest(request);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        private string GetWarehousePhoneNumber(Guid guid)
        {
            Outpost warehouse = queryOutposts.GetWarehouse(guid);
            if (warehouse != null)
            {
                Contact contact = queryContact.Query().Where(c => c.Outpost.Id == warehouse.Id && c.IsMainContact && c.ContactType == Contact.MOBILE_NUMBER_CONTACT_TYPE).FirstOrDefault();
                if (contact != null)
                    return contact.ContactDetail;
            }
            return null;
        }

        public string CreateMessageToBeSentToDispensary(MessageFromDrugShop messageFromDrugShop)
        {
            string OutpostName = queryOutposts.GetOutpostName(messageFromDrugShop.OutpostId);

            string message = messageFromDrugShop.IDCode + " " + messageFromDrugShop.SentDate.ToString("ddMMyy") + OutpostName;
            message = message + " " + messageFromDrugShop.Initials + messageFromDrugShop.BirthDate.ToString("ddMMyy") + messageFromDrugShop.Gender;

            foreach (var service in messageFromDrugShop.ServicesNeeded)
            {
                message = message + " " + service.Code;
            }

            return message;
        }

        private string CreatePostData(ResponseModel model)
        {
            string response = "<?xml version='1.0' encoding='UTF-8'?><sms-response login='" + AppSettings.SmsGatewayUserName + "' password='" + AppSettings.SmsGatewayPassword + "' delivery-notification-requested='true' version='1.0'> ";
            response += "<message id='" + model.Id + "' ref-id='" + model.RefId + "' msisdn='" + model.PhoneNumber + "' service-number='" + model.ServiceNo + "' operator='" + model.Operator + "' ";
            response += "validity-period='" + model.Valability + "' priority='" + model.Priority + "'>";
            response += "<content type='text/plain'>" + model.Content + "</content></message></sms-response>";

            return response;
        }
    }
}