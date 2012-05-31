using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Controllers;
using Core.Persistence;
using Domain;
using Persistence.Queries.Outposts;
using Rhino.Mocks;
using Web.Services;

namespace Tests.Unit.Controllers.SmsRequestControllerTests
{
    public class ObjectMother
    {
        public SmsRequestController controller;

        public ISaveOrUpdateCommand<RawSmsReceived> saveCommandRawSmsReceived;
        public ISaveOrUpdateCommand<MessageFromDrugShop> saveCommandMessageFromDrugShop;
        public ISaveOrUpdateCommand<MessageFromDispensary> saveCommandMessageFromDispensary;
        public IManageReceivedSmsService manageReceivedSmsService;
        public ISmsRequestService smsRequestService;

        public const string WRONGPHONENUMBER = "000000000";
        public const string SERVICENUMBER = "1234";
        public const string TEXT = "";
        public const string CORRECTPHONENUMBER = "0123456789";
        public const string OPERATOR = "Operator";
        public const string DATE = "2008-10-09%2013:30:10";

        public Guid contactId;
        public Contact contact;
        public Guid outpostId;
        public Outpost outpost;
        public Guid rawSmsNotParsedCorrectId;
        public RawSmsReceived rawSmsNotParsedCorrect;


        public void Init()
        {
            MockServices();
            Setup_Controller();
            SetUp_StubData();
        }

        private void SetUp_StubData()
        {
            contactId = Guid.NewGuid();
            contact = MockRepository.GeneratePartialMock<Contact>();
            contact.Stub(c => c.Id).Return(contactId);
            contact.IsMainContact = true;

            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(c => c.Id).Return(outpostId);
            outpost.Contacts = new Contact[] { contact };

            rawSmsNotParsedCorrectId = Guid.NewGuid();
            rawSmsNotParsedCorrect = MockRepository.GeneratePartialMock<RawSmsReceived>();
            rawSmsNotParsedCorrect.Stub(c => c.Id).Return(rawSmsNotParsedCorrectId);
            rawSmsNotParsedCorrect.ParseSucceeded = false;
        }

        private void MockServices()
        {
            saveCommandRawSmsReceived = MockRepository.GenerateMock<ISaveOrUpdateCommand<RawSmsReceived>>();
            saveCommandMessageFromDrugShop = MockRepository.GenerateMock<ISaveOrUpdateCommand<MessageFromDrugShop>>();
            saveCommandMessageFromDispensary = MockRepository.GenerateMock<ISaveOrUpdateCommand<MessageFromDispensary>>();

            manageReceivedSmsService = MockRepository.GenerateMock<IManageReceivedSmsService>();
            smsRequestService = MockRepository.GenerateMock<ISmsRequestService>();
        }

        private void Setup_Controller()
        {
            controller = new SmsRequestController();
            controller.SaveCommandMessageFromDispensary = saveCommandMessageFromDispensary;
            controller.SaveCommandMessageFromDrugShop = saveCommandMessageFromDrugShop;
            controller.SaveCommandRawSmsReceived = saveCommandRawSmsReceived;
            controller.ManageReceivedSmsService = manageReceivedSmsService;
            controller.SmsRequestService = smsRequestService;
        }

        
    }
}
