﻿using System;
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

        public const string WRONGPHONENUMBER = "00000000012";
        public const string CORRECTPHONENUMBER = "0123456789";

        public Guid contactId;
        public Contact contact;
        public Guid outpostId;
        public Outpost outpost;
        public Guid messageFromDrugShopId;
        public MessageFromDrugShop messageFromDrugShop;
        public Guid messageFromDispensaryId;
        public MessageFromDispensary messageFromDispensary;

        public Guid rawSmsInvalidPhoneNumberId;
        public RawSmsReceived rawSmsInvalidPhoneNumber;

        public Guid rawSmsIncorectFormatDrugShopId;
        public RawSmsReceived rawSmsIncorectFormatDrugShop;

        public Guid rawSmsCorerctFormatDrugShopId;
        public RawSmsReceived rawSmsCorerctFormatDrugShop;

        public Guid rawSmsIncorectFormatDispensaryId;
        public RawSmsReceived rawSmsIncorectFormatDispensary;

        public Guid rawSmsCorerctFormatDispensaryId;
        public RawSmsReceived rawSmsCorerctFormatDispensary;
        
        public string XMLStringInvalidPhoneNumber = "<?xml version='1.0' encoding='UTF-8'?><sms-request version='1.0'><message id='54321' submit-date='2008-10-13 13:30:10' msisdn='00000000012' service-number='1234' operator='operator-smpp' operator_id='100' keyword='This' message-count='1'> <content type='text/plain'>This is a test message</content> </message> </sms-request>";
        public string XMLStringFromDrugShop = "<?xml version='1.0' encoding='UTF-8'?><sms-request version='1.0'><message id='54321' submit-date='2008-10-13 13:30:10' msisdn='00000000012' service-number='1234' operator='operator-smpp' operator_id='100' keyword='This' message-count='1'> <content type='text/plain'>This is a test message</content> </message> </sms-request>";
        public string XMLStringDispensary = "<?xml version='1.0' encoding='UTF-8'?><sms-request version='1.0'><message id='54321' submit-date='2008-10-13 13:30:10' msisdn='00000000012' service-number='1234' operator='operator-smpp' operator_id='100' keyword='This' message-count='1'> <content type='text/plain'>This is a test message</content> </message> </sms-request>";


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

            messageFromDrugShopId = Guid.NewGuid();
            messageFromDrugShop = MockRepository.GeneratePartialMock<MessageFromDrugShop>();
            messageFromDrugShop.Stub(c => c.Id).Return(messageFromDrugShopId);
            messageFromDrugShop.OutpostId = outpostId;
            messageFromDrugShop.IDCode = "12345678";
            messageFromDrugShop.Initials = "XY";
            messageFromDrugShop.Gender = "M";

            messageFromDispensaryId = Guid.NewGuid();
            messageFromDispensary = MockRepository.GeneratePartialMock<MessageFromDispensary>();
            messageFromDispensary.Stub(c => c.Id).Return(messageFromDispensaryId);
            messageFromDispensary.MessageFromDrugShop = messageFromDrugShop;
            messageFromDispensary.OutpostId = outpostId;
            messageFromDispensary.OutpostType = 1;


            rawSmsInvalidPhoneNumberId = Guid.NewGuid();
            rawSmsInvalidPhoneNumber = MockRepository.GeneratePartialMock<RawSmsReceived>();
            rawSmsInvalidPhoneNumber.Stub(c => c.Id).Return(rawSmsInvalidPhoneNumberId);
            rawSmsInvalidPhoneNumber.Sender = WRONGPHONENUMBER;
            rawSmsInvalidPhoneNumber.OutpostId = Guid.Empty;
            rawSmsInvalidPhoneNumber.OutpostType = 0;

            rawSmsIncorectFormatDrugShopId = Guid.NewGuid();
            rawSmsIncorectFormatDrugShop = MockRepository.GeneratePartialMock<RawSmsReceived>();
            rawSmsIncorectFormatDrugShop.Stub(c => c.Id).Return(rawSmsIncorectFormatDrugShopId);
            rawSmsIncorectFormatDrugShop.Sender = CORRECTPHONENUMBER;
            rawSmsIncorectFormatDrugShop.OutpostId = outpostId;
            rawSmsIncorectFormatDrugShop.OutpostType = 0;
            rawSmsIncorectFormatDrugShop.Content = "XRT12 0512F SS5";
            rawSmsIncorectFormatDrugShop.ParseSucceeded = false;

            rawSmsCorerctFormatDrugShopId = Guid.NewGuid();
            rawSmsCorerctFormatDrugShop = MockRepository.GeneratePartialMock<RawSmsReceived>();
            rawSmsCorerctFormatDrugShop.Stub(c => c.Id).Return(rawSmsCorerctFormatDrugShopId);
            rawSmsCorerctFormatDrugShop.Sender = CORRECTPHONENUMBER;
            rawSmsCorerctFormatDrugShop.OutpostId = outpostId;
            rawSmsCorerctFormatDrugShop.OutpostType = 0;
            rawSmsCorerctFormatDrugShop.Content = "XRT120572F S1";
            rawSmsCorerctFormatDrugShop.ParseSucceeded = true;

            rawSmsIncorectFormatDispensaryId = Guid.NewGuid();
            rawSmsIncorectFormatDispensary = MockRepository.GeneratePartialMock<RawSmsReceived>();
            rawSmsIncorectFormatDispensary.Stub(c => c.Id).Return(rawSmsIncorectFormatDispensaryId);
            rawSmsIncorectFormatDispensary.Sender = CORRECTPHONENUMBER;
            rawSmsIncorectFormatDispensary.OutpostId = outpostId;
            rawSmsIncorectFormatDispensary.OutpostType = 1;
            rawSmsIncorectFormatDispensary.Content = "124585488 DD1Tr1A2";
            rawSmsIncorectFormatDispensary.ParseSucceeded = false;

            rawSmsCorerctFormatDispensaryId = Guid.NewGuid();
            rawSmsCorerctFormatDispensary = MockRepository.GeneratePartialMock<RawSmsReceived>();
            rawSmsCorerctFormatDispensary.Stub(c => c.Id).Return(rawSmsCorerctFormatDispensaryId);
            rawSmsCorerctFormatDispensary.Sender = CORRECTPHONENUMBER;
            rawSmsCorerctFormatDispensary.OutpostId = outpostId;
            rawSmsCorerctFormatDispensary.OutpostType = 1;
            rawSmsCorerctFormatDispensary.Content = "124585488 D1 T1 A2";
            rawSmsCorerctFormatDispensary.ParseSucceeded = true;
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