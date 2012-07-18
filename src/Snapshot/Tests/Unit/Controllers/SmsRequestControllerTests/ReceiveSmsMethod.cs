﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using System.Web.Mvc;
using Domain;

namespace Tests.Unit.Controllers.SmsRequestControllerTests
{
    [TestFixture]
    public class ReceiveSmsMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void WhenPhoneNumberIsIncorect_Itshould_SaveTheRawSms_AndSendSmsBack_WithErrorMessage()
        {
            //Arrange
            objectMother.manageReceivedSmsService.Expect(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(Arg<RawSmsReceived>.Is.Anything)).Return(objectMother.rawSmsInvalidPhoneNumber);
            objectMother.smsRequestService.Expect(call => call.SendMessage(Arg<string>.Is.Anything, Arg<RawSmsReceived>.Is.Anything)).Return(true);
            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
                p => p.OutpostId == Guid.Empty &&
                     p.Sender == ObjectMother.WRONGPHONENUMBER &&
                     p.ParseSucceeded == false &&
                     p.ParseErrorMessage == "Phone number is not valid."
                     )));

            //Act
            var result = objectMother.controller.ReceiveSms(ObjectMother.CORRECTMESSAGEFROMDRUGSHOP, ObjectMother.WRONGPHONENUMBER);

            //Assert
            objectMother.manageReceivedSmsService.VerifyAllExpectations();
            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
            objectMother.smsRequestService.VerifyAllExpectations();
            var res = result as EmptyResult;
            Assert.IsNotNull(res);

        }

        [Test]
        public void WhenMessageIsFromDrugShop_ButContentIsNotCorrect_Itshould_SaveTheRawSms_AndSendSmsBack_WithErrorMessage()
        {
            //Arrange
            objectMother.manageReceivedSmsService.Expect(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(Arg<RawSmsReceived>.Is.Anything)).Return(objectMother.rawSmsIncorectFormatDrugShop);
            objectMother.manageReceivedSmsService.Expect(call => call.ParseRawSmsReceivedFromDrugShop(objectMother.rawSmsIncorectFormatDrugShop)).Return(objectMother.rawSmsIncorectFormatDrugShop);
            objectMother.smsRequestService.Expect(call => call.SendMessage(Arg<string>.Is.Anything, Arg<RawSmsReceived>.Is.Anything)).Return(true);
            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
                p => p.OutpostId == objectMother.outpostId &&
                     p.Sender == ObjectMother.CORRECTPHONENUMBER &&
                     p.ParseSucceeded == false
                     )));

            //Act
            var result = objectMother.controller.ReceiveSms(ObjectMother.INCORRECTMESSAGEFROMDRUGSHOP, ObjectMother.CORRECTPHONENUMBER);

            //Assert
            objectMother.manageReceivedSmsService.VerifyAllExpectations();
            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
            objectMother.smsRequestService.VerifyAllExpectations();
            var res = result as EmptyResult;
            Assert.IsNotNull(res);
        }

        [Test]
        public void WhenMessageIsFromDispensary_ButContentIsNotCorrect_Itshould_SaveTheRawSms_AndSendSmsBack_WithErrorMessage()
        {
            //Arrange
            objectMother.manageReceivedSmsService.Expect(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(Arg<RawSmsReceived>.Is.Anything)).Return(objectMother.rawSmsIncorectFormatDispensary);
            objectMother.manageReceivedSmsService.Expect(call => call.ParseRawSmsReceivedFromDispensary(objectMother.rawSmsIncorectFormatDispensary)).Return(objectMother.rawSmsIncorectFormatDispensary);
            objectMother.smsRequestService.Expect(call => call.SendMessage(Arg<string>.Is.Anything, Arg<RawSmsReceived>.Is.Anything)).Return(true);
            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
                p => p.OutpostId == objectMother.outpostId &&
                     p.Sender == ObjectMother.CORRECTPHONENUMBER &&
                     p.ParseSucceeded == false
                     )));

            //Act
            var result = objectMother.controller.ReceiveSms(ObjectMother.INCORRECTMESSAGEFROMDISPENSARY, ObjectMother.CORRECTPHONENUMBER);

            //Assert
            objectMother.manageReceivedSmsService.VerifyAllExpectations();
            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
            objectMother.smsRequestService.VerifyAllExpectations();
            var res = result as EmptyResult;
            Assert.IsNotNull(res);
        }

        [Test]
        public void WhenMessageIsFromDrugShop_ContentIsCorrect_Itshould_SaveTheRawSms_AndSendSmsToDispensary_AndSendPasswordToDrugshop()
        {
            //Arrange
            objectMother.manageReceivedSmsService.Expect(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(Arg<RawSmsReceived>.Is.Anything)).Return(objectMother.rawSmsCorerctFormatDrugShop);
            objectMother.manageReceivedSmsService.Expect(call => call.ParseRawSmsReceivedFromDrugShop(objectMother.rawSmsCorerctFormatDrugShop)).Return(objectMother.rawSmsCorerctFormatDrugShop);
            objectMother.manageReceivedSmsService.Expect(call => call.CreateMessageFromDrugShop(objectMother.rawSmsCorerctFormatDrugShop)).Return(objectMother.messageFromDrugShop);
            objectMother.smsRequestService.Expect(call => call.SendMessage(Arg<string>.Is.Anything, Arg<RawSmsReceived>.Is.Anything)).Return(true);
            objectMother.smsRequestService.Expect(call => call.SendMessageToDispensary(Arg<string>.Is.Anything, Arg<RawSmsReceived>.Is.Anything)).Return(true);
            objectMother.manageReceivedSmsService.Expect(call => call.CreateMessageToBeSentToDispensary(Arg<MessageFromDrugShop>.Is.Anything)).Return("Simbad");

            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
                p => p.OutpostId == objectMother.outpostId &&
                     p.Sender == ObjectMother.CORRECTPHONENUMBER &&
                     p.ParseSucceeded == true
                     )));

            objectMother.saveCommandMessageFromDrugShop.Expect(call => call.Execute(Arg<MessageFromDrugShop>.Matches(
                p => p.OutpostId == objectMother.outpostId &&
                     p.Gender == "F" &&
                     p.IDCode == "12345678"
                     )));


            //Act
            var result = objectMother.controller.ReceiveSms(ObjectMother.CORRECTMESSAGEFROMDRUGSHOP, ObjectMother.CORRECTPHONENUMBER);

            //Assert
            objectMother.manageReceivedSmsService.VerifyAllExpectations();
            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
            objectMother.saveCommandMessageFromDrugShop.VerifyAllExpectations();
            objectMother.smsRequestService.VerifyAllExpectations();
            var res = result as EmptyResult;
            Assert.IsNotNull(res);
        }

        [Test]
        public void WhenMessageIsFromDispensary_AndContentIsCorrent_Itshould_SaveTheRawSms()
        {
            //Arrange
            objectMother.manageReceivedSmsService.Expect(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(Arg<RawSmsReceived>.Is.Anything)).Return(objectMother.rawSmsCorerctFormatDispensary);
            objectMother.manageReceivedSmsService.Expect(call => call.ParseRawSmsReceivedFromDispensary(objectMother.rawSmsCorerctFormatDispensary)).Return(objectMother.rawSmsCorerctFormatDispensary);
            objectMother.manageReceivedSmsService.Expect(call => call.CreateMessageFromDispensary(objectMother.rawSmsCorerctFormatDispensary)).Return(objectMother.messageFromDispensary);
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(new MessageFromDispensary[] {objectMother.messageFromDispensary}.AsQueryable());
            objectMother.deleteCommand.Expect(call => call.Execute(Arg<MessageFromDispensary>.Matches(p => p.Id == objectMother.messageFromDispensaryId)));


            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
                p => p.OutpostId == objectMother.outpostId &&
                     p.Sender == ObjectMother.CORRECTPHONENUMBER &&
                     p.ParseSucceeded == true
                     )));

            objectMother.saveCommandMessageFromDispensary.Expect(call => call.Execute(Arg<MessageFromDispensary>.Matches(
                p => p.OutpostId == objectMother.outpostId &&
                     p.MessageFromDrugShop.Id == objectMother.messageFromDrugShop.Id &&
                     p.OutpostType == 1
                     )));


            //Act
            var result = objectMother.controller.ReceiveSms(ObjectMother.CORRECTMESSAGEFROMDISPENSARY, ObjectMother.CORRECTPHONENUMBER);

            //Assert
            objectMother.manageReceivedSmsService.VerifyAllExpectations();
            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
            objectMother.saveCommandMessageFromDispensary.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();
            objectMother.deleteCommand.VerifyAllExpectations();

            var res = result as EmptyResult;
            Assert.IsNotNull(res);
        }


    }
}
