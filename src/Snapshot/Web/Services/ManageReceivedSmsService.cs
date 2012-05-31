using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using System.Text.RegularExpressions;
using System.Globalization;
using Core.Persistence;
using Persistence.Queries.Outposts;

namespace Web.Services
{
    public class ManageReceivedSmsService : IManageReceivedSmsService
    {
        private const string CONTENT_FROMDRUGSHOP_REGEX = @"^([A-Za-z]{2,4}[0-9]{6}[M,F,m,f][ \t]+[A-Za-z0-9 +-;/]+)$";
        private const string CONTENT_FROMDISPENSARY_REGEX = @"^([0-9]{8}[A-Za-z0-9 +-;]+)$";
        private const string DateFormat = "ddMMyy";
        private IFormatProvider FormatProvider = CultureInfo.InvariantCulture;

        private IQueryService<ServiceNeeded> queryServiceNeeded;
        private IQueryService<Diagnosis> queryDiagnosis;
        private IQueryService<Treatment> queryTreatment;
        private IQueryService<Advice> queryAdvice;
        private IQueryService<MessageFromDrugShop> queryMessageFromDrugShop;
        private IQueryService<Contact> queryServiceContact;
        private IQueryOutposts queryOutposts;

        public ManageReceivedSmsService(IQueryService<ServiceNeeded> queryServiceNeeded,
                                IQueryService<Diagnosis> queryDiagnosis,
                                IQueryService<Treatment> queryTreatment,
                                IQueryService<Advice> queryAdvice,
                                IQueryService<MessageFromDrugShop> queryMessageFromDrugShop,
                                IQueryService<Contact> queryServiceContact,
                                IQueryOutposts queryOutposts)
        {
            this.queryAdvice = queryAdvice;
            this.queryDiagnosis = queryDiagnosis;
            this.queryMessageFromDrugShop = queryMessageFromDrugShop;
            this.queryOutposts = queryOutposts;
            this.queryServiceContact = queryServiceContact;
            this.queryServiceNeeded = queryServiceNeeded;
            this.queryTreatment = queryTreatment;
        }


        public Guid AssignOutpostToRawSmsReceivedBySenderNumber(Domain.RawSmsReceived rawSmsReceived)
        {
            string number = rawSmsReceived.Sender;
            Contact contact = queryServiceContact.Query().Where(
                c => c.ContactType.Equals(Contact.MOBILE_NUMBER_CONTACT_TYPE) && c.ContactDetail.Contains(number)).FirstOrDefault();
            Outpost outpost = queryOutposts.GetAllContacts().Where(o => o.Contacts.Contains(contact)).FirstOrDefault();
            if (outpost != null)
                return outpost.Id;

            return Guid.Empty;
        }

        public RawSmsReceived ParseRawSmsReceivedFromDrugShop(RawSmsReceived rawSmsReceived)
        {
            Regex regexFromDrugshop = new Regex(CONTENT_FROMDRUGSHOP_REGEX);

            if (regexFromDrugshop.IsMatch(rawSmsReceived.Content))
            {
                string[] parsedLine = rawSmsReceived.Content.Trim().Split(' ');
                string stringDate = parsedLine[0].Substring(parsedLine[0].Length - 7, 6);
                DateTime dateRetult;

                if (!DateTime.TryParseExact(stringDate, DateFormat, FormatProvider, DateTimeStyles.None, out dateRetult))
                {
                    rawSmsReceived.ParseSucceeded = false;
                    rawSmsReceived.ParseErrorMessage = "Date " + stringDate.Trim() + " is incorect. Please check and retry. Thank you.";
                    return rawSmsReceived;
                }

                for (var i = 1; i < parsedLine.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(parsedLine[i]))
                    {
                        var existServiceNeeded = queryServiceNeeded.Query().Where(it => it.Code == parsedLine[i].Trim()).Any();
                        if (!existServiceNeeded)
                        {
                            rawSmsReceived.ParseSucceeded = false;
                            rawSmsReceived.ParseErrorMessage = "Service " + parsedLine[i].Trim() + " is incorect. Please check and retry. Thank you.";
                            return rawSmsReceived;
                        }
                    }
                }

                rawSmsReceived.ParseSucceeded = true;
                return rawSmsReceived;
            }

            rawSmsReceived.ParseSucceeded = false;
            rawSmsReceived.ParseErrorMessage = "The format of your message is incorrect. Please check and retry. Thank you.";
            return rawSmsReceived;
        }

        public Domain.RawSmsReceived ParseRawSmsReceivedFromDispensary(Domain.RawSmsReceived rawSmsReceived)
        {
            //XY150697F RX1 RX2                 ^([A-Za-z]{2}[0-9]{6}[M,F,m,f][ \t]+[A-Za-z0-9 +-;]+)$
            //123432144 TR1 TR2                 ^([0-9]{9}[A-Za-z0-9 +-;]+)$
            Regex regexFromDispensary = new Regex(CONTENT_FROMDISPENSARY_REGEX);

            if (regexFromDispensary.IsMatch(rawSmsReceived.Content))
            {
                string[] parsedLine = rawSmsReceived.Content.Trim().Split(' ');
                string IdCode = parsedLine[0].Substring(0, 8);
                if (!queryMessageFromDrugShop.Query().Where(it => it.IDCode == IdCode).Any())
                {
                    rawSmsReceived.ParseSucceeded = false;
                    rawSmsReceived.ParseErrorMessage = "ID code " + IdCode + " is incorect. Please check and retry. Thank you.";
                    return rawSmsReceived;
                }

                for (var i = 1; i < parsedLine.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(parsedLine[i]))
                    {
                        var existDiagnosis = queryDiagnosis.Query().Where(it => it.Code == parsedLine[i]).Any();
                        var existTreatment = queryTreatment.Query().Where(it => it.Code == parsedLine[i]).Any();
                        var existAdvice = queryAdvice.Query().Where(it => it.Code == parsedLine[i]).Any();

                        if (!existDiagnosis && !existTreatment && !existAdvice)
                        {
                            rawSmsReceived.ParseSucceeded = false;
                            rawSmsReceived.ParseErrorMessage = "Service " + parsedLine[i] + " is incorect. Please check and retry. Thank you.";
                            return rawSmsReceived;
                        }
                    }
                }

                rawSmsReceived.ParseSucceeded = true;
                return rawSmsReceived;
            }

            rawSmsReceived.ParseSucceeded = false;
            rawSmsReceived.ParseErrorMessage = "The format of your message is incorrect. Please check and retry. Thank you.";
            return rawSmsReceived;
        }

        public Domain.MessageFromDrugShop CreateMessageFromDrugShop(Domain.RawSmsReceived rawSmsReceived)
        {
            MessageFromDrugShop message = new MessageFromDrugShop();

            string[] parsedLine = rawSmsReceived.Content.Trim().Split(' ');
            message.Gender = parsedLine[0].Substring(parsedLine[0].Length - 1, 1);
            string stringDate = parsedLine[0].Substring(parsedLine[0].Length - 7, 6);
            message.BirthDate = DateTime.ParseExact(stringDate, DateFormat, FormatProvider, DateTimeStyles.None);
            message.Initials = parsedLine[0].Substring(0, parsedLine[0].Length - 7);
            message.IDCode = GenerateIDCode();
            message.OutpostId = rawSmsReceived.OutpostId;
            message.SentDate = rawSmsReceived.ReceivedDate;

            for (var i = 1; i < parsedLine.Count(); i++)
            {
                if (!string.IsNullOrEmpty(parsedLine[i]))
                {
                    ServiceNeeded serviceNeeded = queryServiceNeeded.Query().Where(it => it.Code == parsedLine[i].Trim()).FirstOrDefault();
                    message.AddServiceNeeded(serviceNeeded);
                }
            }

            return message;
        }

        public MessageFromDispensary CreateMessageFromDispensary(Domain.RawSmsReceived rawSmsReceived)
        {
            MessageFromDispensary message = new MessageFromDispensary();

            string[] parsedLine = rawSmsReceived.Content.Trim().Split(' ');
            string IdCode = parsedLine[0].Substring(0, 8);

            message.OutpostId = rawSmsReceived.OutpostId;
            message.MessageFromDrugShop = queryMessageFromDrugShop.Query().Where(it => it.IDCode == IdCode).FirstOrDefault();
            message.SentDate = rawSmsReceived.ReceivedDate;

            for (var i = 1; i < parsedLine.Count(); i++)
            {
                if (!string.IsNullOrEmpty(parsedLine[i]))
                {
                    var diagnosis = queryDiagnosis.Query().Where(it => it.Code == parsedLine[i]).FirstOrDefault();
                    var treatment = queryTreatment.Query().Where(it => it.Code == parsedLine[i]).FirstOrDefault();
                    var advice = queryAdvice.Query().Where(it => it.Code == parsedLine[i]).FirstOrDefault();

                    if (diagnosis != null)
                        message.Diagnosises.Add(diagnosis);
                    if (treatment != null)
                        message.Treatments.Add(treatment);
                    if (advice != null)
                        message.Advices.Add(advice);
                }
            }

            return message;
        }

        private string GenerateIDCode()
        {
            string code = Guid.NewGuid().ToString().Substring(0, 8);
            while (queryMessageFromDrugShop.Query().Where(it => it.IDCode == code).Any())
            {
                code = Guid.NewGuid().ToString().Substring(0, 8);
            }
            return code;

        }
    }
}