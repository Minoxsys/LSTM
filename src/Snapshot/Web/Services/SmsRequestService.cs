using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using Persistence.Queries.Outposts;
using Core.Persistence;
using Web.Models.SmsRequest;
using Web.Bootstrap;
using System.Security.Cryptography;

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
            model.Id = response.SmsId;
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
            model.Id = Get7Digits();
            model.Content = CreateMessageToBeSentToDispensary(message);
            model.DeferDate = "";
            model.Operator = "";
            model.PhoneNumber = GetWarehousePhoneNumber(message.OutpostId);
            model.Priority = "1";
            model.RefId = rawSms.SmsId;
            model.ServiceNo = rawSms.ServiceNumber;
            model.Valability = "3";

            string request = CreateDispensaryData(model);

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

        public bool SendResponseMessage(RawSmsReceived rawSmsReceived)
        {
            string request = CreateEmptyPostData(rawSmsReceived);

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

        private string CreateEmptyPostData(RawSmsReceived rawSmsReceived)
        {
            string response = "<?xml version='1.0' encoding='UTF-8'?><sms-response login='" + AppSettings.SmsGatewayUserName + "' password='" + AppSettings.SmsGatewayPassword + "' version='1.0'> ";
            return response;
        }

        private string CreateDispensaryData(ResponseModel model)
        {
            string bulkRequest = "<?xml version='1.0' encoding='UTF-8'?>";
            bulkRequest += "<bulk-request login='" + AppSettings.SmsGatewayUserName + "' password='" + AppSettings.SmsGatewayPassword + "' request-id='"+model.Id+"' delivery-notification-requested='true' version='1.0'>";
            bulkRequest += "<message id='1' msisdn='"+model.PhoneNumber+"' service-number='" + AppSettings.SmsGatewayShortcode + "' validity-period='"+model.Valability+"' priority='"+model.Priority+"'> ";
            bulkRequest += "<content type='text/plain'>"+model.Content+"</content>";
            bulkRequest += "</message></bulk-request>";

            return bulkRequest;
        }

        private string Get7Digits()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            int random = BitConverter.ToInt32(bytes, 0) % 10000000;
            return String.Format("{0:D7}", random);
        }

    }
}