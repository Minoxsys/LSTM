using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using Persistence.Queries.Outposts;
using Core.Persistence;

namespace Web.Services
{
    public class SmsGatewayService : ISmsGatewayService
    {
        private const string CONTENT_FROMDRUGSHOP_REGEX = @"^([A-Za-z]{2}[0-9]{6}[M,F,m,f][ \t]+[A-Za-z0-9 +-;/]+)$";
        private const string CONTENT_FROMDISPENSARY_REGEX = @"^([0-9]{9}[A-Za-z0-9 +-;]+)$";

        private IHttpService httpService;
        private ISmsGatewaySettingsService smsGatewaySettingsService;
        private IQueryOutposts queryOutposts;
        private IQueryService<Diagnosis> queryDiagnosis;

        private IQueryService<Contact> queryServiceContact;

        public SmsGatewayService(ISmsGatewaySettingsService smsGatewaySettingsService, IHttpService httpService, IQueryOutposts queryOutposts, IQueryService<Contact> queryServiceContact, IQueryService<Diagnosis> queryDiagnosis)
        {
            this.httpService = httpService;
            this.smsGatewaySettingsService = smsGatewaySettingsService;
            this.queryOutposts = queryOutposts;
            this.queryServiceContact = queryServiceContact;
            this.queryDiagnosis = queryDiagnosis;
        }

        public string SendSmsRequest(SmsRequest smsRequest)
        {
            string postData = GetPostDataFromSettingsAndSmsRequest(smsRequest);
            string postResponse = httpService.Post(smsGatewaySettingsService.SmsGatewayUrl, postData);
            return postResponse;
        }

        private string GetPostDataFromSettingsAndSmsRequest(SmsRequest smsRequest)
        {
            return GetPostDataForSmsRequestFromSettings() + "&" + GetPostDataFromSmsRequest(smsRequest);
        }

        private string GetPostDataForSmsRequestFromSettings()
        {
            StringBuilder postDataBuilder = new StringBuilder();
            postDataBuilder.Append("uname=" + smsGatewaySettingsService.SmsGatewayUserName);
            postDataBuilder.Append("&pword=" + smsGatewaySettingsService.SmsGatewayPassword);
            postDataBuilder.Append("&from=" + smsGatewaySettingsService.SmsGatewayFrom);
            postDataBuilder.Append("&test=" + smsGatewaySettingsService.SmsGatewayTestMode);
            postDataBuilder.Append("&info=" + smsGatewaySettingsService.SmsGatewayDebugMode);

            return postDataBuilder.ToString();
        }

        private string GetPostDataFromSmsRequest(SmsRequest smsRequest)
        {
            StringBuilder postDataBuilder = new StringBuilder();
            postDataBuilder.Append("selectednums=" + smsRequest.Number);
            postDataBuilder.Append("&custom=" + smsRequest.Id);
            postDataBuilder.Append("&message=" + HttpUtility.HtmlEncode(smsRequest.Message));

            return postDataBuilder.ToString();
        }

        public RawSmsReceived AssignOutpostToRawSmsReceivedBySenderNumber(RawSmsReceived rawSmsReceived)
        {
            string number = rawSmsReceived.Sender;
            Contact contact = queryServiceContact.Query().Where(
                c => c.ContactType.Equals(Contact.MOBILE_NUMBER_CONTACT_TYPE) && c.ContactDetail.Contains(number)).FirstOrDefault();
            Outpost outpost = queryOutposts.GetAllContacts().Where(o => o.Contacts.Contains(contact)).FirstOrDefault();
            if (outpost != null)
            {
                rawSmsReceived.OutpostId = outpost.Id;
                rawSmsReceived.OutpostName = outpost.Name;
            }

            return rawSmsReceived;
        }

        public RawSmsReceived ParseRawSmsReceived(RawSmsReceived rawSmsReceived)
        {
            //XY150697F RX1 RX2                 ^([A-Za-z]{2}[0-9]{6}[M,F,m,f][ \t]+[A-Za-z0-9 +-;]+)$
            //123432144 TR1 TR2                 ^([0-9]{9}[A-Za-z0-9 +-;]+)$

            Regex regexFromDrugshop   = new Regex(CONTENT_FROMDRUGSHOP_REGEX);
            Regex regexFromDispensary = new Regex(CONTENT_FROMDISPENSARY_REGEX);

            if (regexFromDrugshop.IsMatch(rawSmsReceived.Content))
            {
                //getData from message content
                string initials = rawSmsReceived.Content.Substring(0, 2);
                string stringDate = rawSmsReceived.Content.Substring(2, 6);
                string gender = rawSmsReceived.Content.Substring(8, 1);
                string[] diagnosis = rawSmsReceived.Content.Substring(10).Trim().Split(' ');

                foreach (var diagnostic in diagnosis)
                {
                    if (!string.IsNullOrEmpty(diagnostic))
                    {
                        var existDiagnosis = queryDiagnosis.Query().Where(it => it.Code == diagnostic).Any();
                        if (!existDiagnosis)
                        {
                            rawSmsReceived.ParseSucceeded = false;
                            rawSmsReceived.ParseErrorMessage = "Diagnosis " + diagnostic + " does not exist!";
                            return rawSmsReceived;
                        }
                    }
                }
                //validate diagnosis
                //generate message idcode
                //compose message (initials, birthDate, gender, diagnosis, codeId, sentDate, outpostId, rawsmsid)
                //save message
                //send message to dispensary

                rawSmsReceived.ParseSucceeded = true;
                return rawSmsReceived;
            }else

            if (regexFromDispensary.IsMatch(rawSmsReceived.Content))
            {
                //getData from content
                string messageId = rawSmsReceived.Content.Substring(0, 9);
                string[] treatments = rawSmsReceived.Content.Substring(10).Trim().Split(' ');

                //validate treatments
                //get message with Id
                //compose message (codeId, treatment, sentDate, outpostId, rawsmsid)
                //save message
            }

            else
            {
                //no format
            }





            //RawSmsReceivedParseResult parseResult = new RawSmsReceivedParseResult();
            //SmsReceived smsReceived = new SmsReceived();

            //smsReceived.Number = rawSmsReceived.Sender;

            //if (!string.IsNullOrEmpty(contentFragments.ProductStockLevels))
            //{
            //    smsReceived.RawSmsReceivedId = rawSmsReceived.Id;
            //    smsReceived.ProductGroupReferenceCode = GetProductGroupReferenceCodeFromSplitContent(contentFragments);
            //    smsReceived.ReceivedStockLevels = GetReceivedStockLevelsFromContent(contentFragments);

            //    parseResult.ParseSucceeded = true;
            //    parseResult.SmsReceived = smsReceived;
            //}
            //else
            //{
            //    parseResult.ParseSucceeded = false;
            //    parseResult.ParseErrorMessage = "Sms was not parsed correctly!";
            //}
            return rawSmsReceived;
        }

    }
}