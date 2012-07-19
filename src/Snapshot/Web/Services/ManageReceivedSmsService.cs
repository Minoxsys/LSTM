using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using System.Text.RegularExpressions;
using System.Globalization;
using Core.Persistence;
using Persistence.Queries.Outposts;
using System.Xml;
using Web.Bootstrap;

namespace Web.Services
{
    public class ManageReceivedSmsService : IManageReceivedSmsService
    {
        private const string CONTENT_FROMDRUGSHOP_REGEX = @"^(([A-Za-z]{4}[ \t]+)*[A-Za-z]{2,4}[0-9]{6}[M,F,m,f][ \t]+[A-Za-z0-9 +-;/]+)$";
        private const string CONTENT_FROMDISPENSARY_REGEX = @"^(([A-Za-z]{4}[ \t]+)*[0-9A-Za-z]{8}[ \t][A-Za-z0-9 +-;/]+)$";
        private const string DateFormat = "ddMMyy";
        private const string XMLDateFormat = "yyyy-MM-dd HH:mm:ss";
        private IFormatProvider FormatProvider = CultureInfo.InvariantCulture;
        private string KEYWORD = "AFYA";
        private string REFUSEDCODE = "RR";

        private IQueryService<Condition> queryCondition;
        private IQueryService<Diagnosis> queryDiagnosis;
        private IQueryService<Treatment> queryTreatment;
        private IQueryService<Advice> queryAdvice;
        private IQueryService<MessageFromDrugShop> queryMessageFromDrugShop;
        private IQueryService<Contact> queryServiceContact;
        private IQueryOutposts queryOutposts;
        private IQueryService<Appointment> queryAppointment;
        private IQueryService<WrongMessage> queryWrongMessage;

        public ManageReceivedSmsService(IQueryService<Condition> queryCondition,
                                IQueryService<Diagnosis> queryDiagnosis,
                                IQueryService<Treatment> queryTreatment,
                                IQueryService<Advice> queryAdvice,
                                IQueryService<MessageFromDrugShop> queryMessageFromDrugShop,
                                IQueryService<Contact> queryServiceContact,
                                IQueryOutposts queryOutposts,
                                IQueryService<Appointment> queryAppointment,
                                IQueryService<WrongMessage> queryWrongMessage)
        {
            this.queryAdvice = queryAdvice;
            this.queryDiagnosis = queryDiagnosis;
            this.queryMessageFromDrugShop = queryMessageFromDrugShop;
            this.queryOutposts = queryOutposts;
            this.queryServiceContact = queryServiceContact;
            this.queryCondition = queryCondition;
            this.queryTreatment = queryTreatment;
            this.queryAppointment = queryAppointment;
            this.queryWrongMessage = queryWrongMessage;
        }


        public RawSmsReceived AssignOutpostToRawSmsReceivedBySenderNumber(Domain.RawSmsReceived rawSmsReceived)
        {
            string number = rawSmsReceived.Sender;
            Contact contact = queryServiceContact.Query().Where(
                c => c.ContactType.Equals(Contact.MOBILE_NUMBER_CONTACT_TYPE) && c.ContactDetail.Contains(number)).FirstOrDefault();
            Outpost outpost = queryOutposts.GetAllContacts().Where(o => o.Contacts.Contains(contact)).FirstOrDefault();
            if (outpost != null)
            {
                rawSmsReceived.OutpostId = outpost.Id;
                rawSmsReceived.OutpostType = outpost.OutpostType.Type;
                return rawSmsReceived;
            }

            rawSmsReceived.OutpostId = Guid.Empty;
            rawSmsReceived.OutpostType = 0;
            return rawSmsReceived;
        }

        public RawSmsReceived ParseRawSmsReceivedFromDrugShop(RawSmsReceived rawSmsReceived)
        {
            Regex regexFromDrugshop = new Regex(CONTENT_FROMDRUGSHOP_REGEX);

            if (regexFromDrugshop.IsMatch(rawSmsReceived.Content))
            {
                string[] parsedLine = rawSmsReceived.Content.Trim().Split(' ');
                var index = 0;
                if (parsedLine[0].ToUpper().Contains(KEYWORD))
                    index = 1;
                string stringDate = parsedLine[index].Substring(parsedLine[index].Length - 7, 6);
                DateTime dateRetult;

                if (!DateTime.TryParseExact(stringDate, DateFormat, FormatProvider, DateTimeStyles.None, out dateRetult))
                {
                    rawSmsReceived.ParseSucceeded = false;
                    rawSmsReceived.ParseErrorMessage = "Date " + stringDate.Trim() + " is incorect.";
                    return rawSmsReceived;
                }

                for (var i = index + 1; i < parsedLine.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(parsedLine[i]))
                    {
                        var existCondition = queryCondition.Query().Where(it => it.Code.ToUpper() == parsedLine[i].Trim().ToUpper()).Any();
                        var existAppointment = queryAppointment.Query().Where(it => it.Code.ToUpper() == parsedLine[i].Trim().ToUpper()).Any();

                        if (!existCondition && !existAppointment)
                        {
                            rawSmsReceived.ParseSucceeded = false;
                            rawSmsReceived.ParseErrorMessage = "Service " + parsedLine[i].Trim() + " is incorect.";
                            return rawSmsReceived;
                        }
                    }
                }

                rawSmsReceived.ParseSucceeded = true;
                return rawSmsReceived;
            }

            rawSmsReceived.ParseSucceeded = false;
            rawSmsReceived.ParseErrorMessage = "The format of the message is incorrect.";
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
                var index = 0;
                if (parsedLine[0].ToUpper().Contains(KEYWORD))
                    index = 1;
                string IdCode = parsedLine[index].Substring(0, 8);
                if (!queryMessageFromDrugShop.Query().Where(it => it.IDCode.ToUpper() == IdCode.ToUpper()).Any())
                {
                    rawSmsReceived.ParseSucceeded = false;
                    rawSmsReceived.ParseErrorMessage = "ID code " + IdCode + " is incorect.";
                    return rawSmsReceived;
                }

                for (var i = index + 1; i < parsedLine.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(parsedLine[i]))
                    {
                        var existDiagnosis = queryDiagnosis.Query().Where(it => it.Code.ToUpper() == parsedLine[i].ToUpper()).Any();
                        var existTreatment = queryTreatment.Query().Where(it => it.Code.ToUpper() == parsedLine[i].ToUpper()).Any();
                        var existAdvice = queryAdvice.Query().Where(it => it.Code.ToUpper() == parsedLine[i].ToUpper()).Any();

                        if (!existDiagnosis && !existTreatment && !existAdvice)
                        {
                            rawSmsReceived.ParseSucceeded = false;
                            rawSmsReceived.ParseErrorMessage = "Service " + parsedLine[i] + " is incorect.";
                            return rawSmsReceived;
                        }
                    }
                }

                rawSmsReceived.ParseSucceeded = true;
                return rawSmsReceived;
            }

            rawSmsReceived.ParseSucceeded = false;
            rawSmsReceived.ParseErrorMessage = "The format of the message is incorrect.";
            return rawSmsReceived;
        }

        public Domain.MessageFromDrugShop CreateMessageFromDrugShop(Domain.RawSmsReceived rawSmsReceived)
        {
            MessageFromDrugShop message = new MessageFromDrugShop();

            string[] parsedLine = rawSmsReceived.Content.Trim().Split(' ');

            var index = 0;
            if (parsedLine[0].ToUpper().Contains(KEYWORD))
                index = 1;

            message.Gender = parsedLine[index].Substring(parsedLine[index].Length - 1, 1).ToUpper();
            string stringDate = parsedLine[index].Substring(parsedLine[index].Length - 7, 6);
            message.BirthDate = DateTime.ParseExact(stringDate, DateFormat, FormatProvider, DateTimeStyles.None);
            message.Initials = parsedLine[index].Substring(0, parsedLine[index].Length - 7).ToUpper();
            message.IDCode = GenerateIDCode();
            message.OutpostId = rawSmsReceived.OutpostId;
            message.SentDate = rawSmsReceived.ReceivedDate;

            for (var i = index + 1; i < parsedLine.Count(); i++)
            {
                var condition = queryCondition.Query().Where(it => it.Code == parsedLine[i]).FirstOrDefault();
                var appointment = queryAppointment.Query().Where(it => it.Code == parsedLine[i]).FirstOrDefault();

                if (condition != null)
                    message.ServicesNeeded.Add(condition);
                if (appointment != null)
                    message.Appointments.Add(appointment);
            }

            return message;
        }

        public MessageFromDispensary CreateMessageFromDispensary(Domain.RawSmsReceived rawSmsReceived)
        {
            MessageFromDispensary message = new MessageFromDispensary();

            string[] parsedLine = rawSmsReceived.Content.Trim().Split(' ');

            var index = 0;
            if (parsedLine[0].ToUpper().Contains(KEYWORD))
                index = 1;

            string IdCode = parsedLine[index].Substring(0, 8);

            message.OutpostId = rawSmsReceived.OutpostId;
            message.OutpostType = rawSmsReceived.OutpostType;
            message.MessageFromDrugShop = queryMessageFromDrugShop.Query().Where(it => it.IDCode == IdCode).FirstOrDefault();
            message.SentDate = rawSmsReceived.ReceivedDate;

            for (var i = index + 1; i < parsedLine.Count(); i++)
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

        public string CreateMessageToBeSentToDispensary(MessageFromDrugShop messageFromDrugShop)
        {
            string OutpostName = queryOutposts.GetOutpostName(messageFromDrugShop.OutpostId);

            string message = messageFromDrugShop.IDCode + " " + messageFromDrugShop.SentDate.ToString("ddMMyy") + OutpostName;
            message = message + " " + messageFromDrugShop.Initials + messageFromDrugShop.BirthDate.ToString("ddMMyy") + messageFromDrugShop.Gender;

            foreach (var service in messageFromDrugShop.ServicesNeeded)
                message = message + " " + service.Code;

            foreach (var service in messageFromDrugShop.Appointments)
                message = message + " " + service.Code;

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


        public bool DoesMessageStartWithKeyword(string message)
        {
            return message.Substring(0, 4).ToUpper() == KEYWORD;
        }

        public bool DoesMessageContainRRCode(MessageFromDrugShop drugshopMessage)
        {
            return drugshopMessage.ServicesNeeded.FirstOrDefault(it => it.Code == REFUSEDCODE) == null;
        }
    }
}