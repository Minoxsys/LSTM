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
using System.Text;

namespace Web.Services
{
    public class SmsRequestService : ISmsRequestService
    {
        private IHttpService httpService;
        private IQueryOutposts queryOutposts;
        private IQueryService<Contact> queryContact;

        private const string DateFormat = "ddMMyy";
        private string URL = AppSettings.SmsGatewayUrl;

        public SmsRequestService(IHttpService httpService, IQueryOutposts queryOutposts, IQueryService<Contact> queryContact)
        {
            this.httpService = httpService;
            this.queryOutposts = queryOutposts;
            this.queryContact = queryContact;
        }

        public bool SendMessage(string message, RawSmsReceived response)
        {
            string phoneNumber = response.Sender.Trim('+');
            string postData = GeneratePostData(message, phoneNumber);

            try
            {
                this.httpService.Post(postData);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SendMessageToDispensary(string message, RawSmsReceived rawSms)
        {
            string phoneNumber = GetWarehousePhoneNumber(rawSms.OutpostId).Trim('+'); ;
            string postData = GeneratePostData(message, phoneNumber);

            try
            {
                this.httpService.Post(postData);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GeneratePostData(string message, string phoneNumber)
        {
            String postMessage = HttpUtility.UrlEncode(message);
            String strPost = "?phonenumber=%2B" + phoneNumber + "&user=" + AppSettings.SmsGatewayUserName + "&password=" + AppSettings.SmsGatewayPassword + "&text=" + postMessage;
            return strPost;
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
    }
}