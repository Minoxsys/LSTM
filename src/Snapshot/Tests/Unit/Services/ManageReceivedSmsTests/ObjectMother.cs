using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Services;
using Core.Persistence;
using Domain;
using Persistence.Queries.Outposts;
using Rhino.Mocks;

namespace Tests.Unit.Services.ManageReceivedSmsTests
{
    public class ObjectMother
    {
        public IManageReceivedSmsService service;

        public IQueryService<Condition> queryCondition;
        public IQueryService<Diagnosis> queryDiagnosis;
        public IQueryService<Treatment> queryTreatment;
        public IQueryService<Advice> queryAdvice;
        public IQueryService<MessageFromDrugShop> queryMessageFromDrugShop;
        public IQueryService<Contact> queryServiceContact;
        public IQueryOutposts queryOutposts;
        public IQueryService<Appointment> queryAppointment;
        public IQueryService<WrongMessage> queryWrongMessage;
        public ISaveOrUpdateCommand<Contact> saveContact;

        public const string MOBILE_NUMBER = "0123456789";
        public const string WRONG_MOBILE_NUMBER = "1245781584";
        public const string IN_NUMBER = "1234";
        public const string OPERATOR = "Operator";
        public const string DATE = "2008-10-10%2013:30:10";
        public const string WRONGCONTENTDRUGSHOP = "AFYA XRTDRDR485478654651354 RETFF";
        public const string WRONGDATEMESSAGEFROMDRUGSHOP = "AFYA XY231398F RX1 RX2";
        public const string WRONGSERVICEMESSAGEFROMDRUGSHOP = "AFYA XY230498F RX1 RX2";
        public const string CORRECTMESSAGEFROMDRUGSHOP = "AFYA XYXX230498F    S1 S5  S6  H1 H2 ";
        public const string WRONGIDCODEMESSAGEDISPENSARY = "AFYA 12343214 TR1 TR2";
        public const string WRONGSERVICECODEFORDISPENSARYMESSAGE = "AFYA 10000008 DIAG1 TREAT1 ADV1";
        public const string CORRECTMESSAGEFROMDISPENSARY = "AFYA 10000008  D1  D2     T4  T3 A1        ";
        public string MESSAGE = "10000008 " + DateTime.UtcNow.AddMonths(-1).ToString("ddMMyy") + " XY" + DateTime.UtcNow.ToString("ddMMyy") + "F";
        public const string MESSAGEFORACTIVATION = "AFYA WEZESHA";

        public Guid contactId;
        public Contact contact;
        public Guid outpostId;
        public Outpost outpost;
        public Guid rawSmsReceivedId;
        public RawSmsReceived rawSmsReceived;
        public Guid messageFromDrugShopId;
        public MessageFromDrugShop messageFromDrugShop;
        public Guid outpostTypeId;
        public OutpostType outpostType;
        public Guid rawSmsReceivedFromDispensaryId;
        public RawSmsReceived rawSmsReceivedFromDispensary;
        public Guid rawSmsReceivedForActivationId;
        public RawSmsReceived rawSmsReceivedForActivation;
        

        public void Init()
        {
            MockServices();
            Setup_Service();
            SetUp_StubData();
        }

        private void SetUp_StubData()
        {
            contactId = Guid.NewGuid();
            contact = MockRepository.GeneratePartialMock<Contact>();
            contact.Stub(c => c.Id).Return(contactId);
            contact.ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE;
            contact.ContactDetail = MOBILE_NUMBER;
            contact.IsMainContact = true;

            outpostTypeId = Guid.NewGuid();
            outpostType = MockRepository.GeneratePartialMock<OutpostType>();
            outpostType.Stub(c => c.Id).Return(outpostTypeId);
            outpostType.Name = "Dispensary";
            outpostType.Type = 1;

            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(c => c.Id).Return(outpostId);
            outpost.Contacts = new Contact[] { contact };
            outpost.OutpostType = outpostType;

            rawSmsReceivedId = Guid.NewGuid();
            rawSmsReceived = MockRepository.GeneratePartialMock<RawSmsReceived>();
            rawSmsReceived.Stub(c => c.Id).Return(rawSmsReceivedId);
            rawSmsReceived.Content = CORRECTMESSAGEFROMDRUGSHOP;
            rawSmsReceived.OutpostType = 1;
            rawSmsReceived.OutpostId = outpostId;
            rawSmsReceived.ParseErrorMessage = null;
            rawSmsReceived.ParseSucceeded = true;
            rawSmsReceived.ReceivedDate = DateTime.UtcNow;
            rawSmsReceived.Sender = MOBILE_NUMBER;

            rawSmsReceivedFromDispensaryId = Guid.NewGuid();
            rawSmsReceivedFromDispensary = MockRepository.GeneratePartialMock<RawSmsReceived>();
            rawSmsReceivedFromDispensary.Stub(c => c.Id).Return(rawSmsReceivedFromDispensaryId);
            rawSmsReceivedFromDispensary.Content = CORRECTMESSAGEFROMDISPENSARY;
            rawSmsReceivedFromDispensary.OutpostType = 1;
            rawSmsReceivedFromDispensary.OutpostId = outpostId;
            rawSmsReceivedFromDispensary.ParseErrorMessage = null;
            rawSmsReceivedFromDispensary.ParseSucceeded = true;
            rawSmsReceivedFromDispensary.ReceivedDate = DateTime.UtcNow;
            rawSmsReceivedFromDispensary.Sender = MOBILE_NUMBER;


            rawSmsReceivedForActivationId = Guid.NewGuid();
            rawSmsReceivedForActivation = MockRepository.GeneratePartialMock<RawSmsReceived>();
            rawSmsReceivedForActivation.Stub(c => c.Id).Return(rawSmsReceivedForActivationId);
            rawSmsReceivedForActivation.Content = MESSAGEFORACTIVATION;
            rawSmsReceivedForActivation.OutpostType = 1;
            rawSmsReceivedForActivation.OutpostId = outpostId;
            rawSmsReceivedForActivation.ParseErrorMessage = null;
            rawSmsReceivedForActivation.ParseSucceeded = true;
            rawSmsReceivedForActivation.ReceivedDate = DateTime.UtcNow;
            rawSmsReceivedForActivation.Sender = MOBILE_NUMBER;

            messageFromDrugShopId = Guid.NewGuid();
            messageFromDrugShop = MockRepository.GeneratePartialMock<MessageFromDrugShop>();
            messageFromDrugShop.Stub(c => c.Id).Return(messageFromDrugShopId);
            messageFromDrugShop.BirthDate = DateTime.UtcNow;
            messageFromDrugShop.Gender = "F";
            messageFromDrugShop.IDCode = "10000008";
            messageFromDrugShop.Initials = "XY";
            messageFromDrugShop.OutpostId = outpostId;
            messageFromDrugShop.SentDate = DateTime.UtcNow.AddMonths(-1);

        }

        private void MockServices()
        {
            queryCondition = MockRepository.GenerateMock<IQueryService<Condition>>();
            queryDiagnosis = MockRepository.GenerateMock<IQueryService<Diagnosis>>();
            queryTreatment = MockRepository.GenerateMock<IQueryService<Treatment>>();
            queryAdvice = MockRepository.GenerateMock<IQueryService<Advice>>();
            queryMessageFromDrugShop = MockRepository.GenerateMock<IQueryService<MessageFromDrugShop>>();
            queryServiceContact = MockRepository.GenerateMock<IQueryService<Contact>>();
            queryOutposts = MockRepository.GenerateMock<IQueryOutposts>();
            queryAppointment = MockRepository.GenerateMock<IQueryService<Appointment>>();
            queryWrongMessage = MockRepository.GenerateMock<IQueryService<WrongMessage>>();
            saveContact = MockRepository.GenerateMock<ISaveOrUpdateCommand<Contact>>();
        }

        private void Setup_Service()
        {
            service = new ManageReceivedSmsService(queryCondition, queryDiagnosis, queryTreatment, queryAdvice, queryMessageFromDrugShop, queryServiceContact, queryOutposts, queryAppointment, queryWrongMessage, saveContact);
        }

        public IQueryable<Condition> ListOfCondition()
        {
            List<Condition> conditionList = new List<Condition>();

            for (int i = 0; i < 10; i++)
            {
                conditionList.Add(new Condition
                {
                    Code = "S" + i,
                    Description = "some Description",
                    Keyword = "K" + i
                });
            }
            return conditionList.AsQueryable();
        }

        public IQueryable<Appointment> ListOfAppointment()
        {
            List<Appointment> list = new List<Appointment>();

            for (int i = 0; i < 10; i++)
            {
                list.Add(new Appointment
                {
                    Code = "H" + i,
                    Description = "some Description",
                    Keyword = "H" + i
                });
            }
            return list.AsQueryable();
        }

        public IQueryable<MessageFromDrugShop> ListOfMessagesFromDrugSho()
        {
            List<MessageFromDrugShop> messageList = new List<MessageFromDrugShop>();

            for (int i = 0; i < 10; i++)
            {
                messageList.Add(new MessageFromDrugShop
                {
                    BirthDate = DateTime.UtcNow.AddDays(-i),
                    Gender = "F",
                    IDCode = (10000000 + i).ToString(),
                    Initials = "XY",
                    OutpostId = outpostId,
                    SentDate = DateTime.UtcNow.AddDays(-i)
                });
            }
            return messageList.AsQueryable();
        }

        public IQueryable<Diagnosis> ListOfDiagnosis()
        {
            List<Diagnosis> list = new List<Diagnosis>();

            for (int i = 0; i < 10; i++)
            {
                list.Add(new Diagnosis
                {
                    Code = "D" + i,
                    Keyword = "Diagnosis " + i,
                    Description = "Some description for D" + i

                });
            }
            return list.AsQueryable();
        }

        public IQueryable<Treatment> ListOfTreatments()
        {
            List<Treatment> list = new List<Treatment>();

            for (int i = 0; i < 10; i++)
            {
                list.Add(new Treatment
                {
                    Code = "T" + i,
                    Keyword = "Treatment " + i,
                    Description = "Some description for T" + i

                });
            }
            return list.AsQueryable();
        }

        public IQueryable<Advice> ListOfAdvices()
        {
            List<Advice> list = new List<Advice>();

            for (int i = 0; i < 10; i++)
            {
                list.Add(new Advice
                {
                    Code = "A" + i,
                    Keyword = "Advice " + i,
                    Description = "Some description for A" + i

                });
            }
            return list.AsQueryable();
        }
    }
}
