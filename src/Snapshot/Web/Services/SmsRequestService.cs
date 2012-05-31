using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using Persistence.Queries.Outposts;
using Core.Persistence;

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

        public bool SendMessage(string message, string number)
        {
            SmsRequest request = new SmsRequest();
            request.Number = number;
            request.Message = message;

            try
            {
                //smsGatewayService.SendSmsRequest(request);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SendMessageToDispensary(MessageFromDrugShop message)
        {
            SmsRequest request = new SmsRequest();
            request.Number = GetWarehousePhoneNumber(message.OutpostId);
            request.Message = CreateMessageToBeSentToDispensary(message);
            if (request.Number != null)
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
    }
}