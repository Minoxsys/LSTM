//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Rhino.Mocks;
//using NUnit.Framework;
//using System.Web.Mvc;
//using Domain;

//namespace Tests.Unit.Controllers.SmsRequestControllerTests
//{
//    [TestFixture]
//    public class ReceiveSmsMethod
//    {
//        public ObjectMother objectMother = new ObjectMother();

//        [SetUp]
//        public void BeforeAll()
//        {
//            objectMother.Init();
//        }

//        [Test]
//        public void WhenPhoneNumberIsIncorect_Itshould_SaveTheRawSms_AndSendSmsBack_WithErrorMessage()
//        {
//            //Arrange
//            objectMother.manageReceivedSmsService.Expect(call => call.GetRawSmsReceivedFromXMLString(objectMother.XMLStringInvalidPhoneNumber)).Return(objectMother.rawSmsInvalidPhoneNumber);
//            objectMother.manageReceivedSmsService.Expect(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(objectMother.rawSmsInvalidPhoneNumber)).Return(objectMother.rawSmsInvalidPhoneNumber);
//            objectMother.smsRequestService.Expect(call => call.SendMessage(Arg<string>.Is.Anything, Arg<RawSmsReceived>.Is.Anything)).Return(true);
//            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
//                p => p.OutpostId == Guid.Empty &&
//                     p.Sender == ObjectMother.WRONGPHONENUMBER &&
//                     p.ParseSucceeded == false &&
//                     p.ParseErrorMessage == "Phone number is not valid."
//                     )));
//            objectMother.MockRequest(objectMother.XMLStringInvalidPhoneNumber);

//            //Act
//            var result = objectMother.controller.ReceiveSms();

//            //Assert
//            objectMother.manageReceivedSmsService.VerifyAllExpectations();
//            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
//            objectMother.smsRequestService.VerifyAllExpectations();
//            var res = result as EmptyResult;
//            Assert.IsNotNull(res);

//        }

//        [Test]
//        public void WhenMessageIsFromDrugShop_ButContentIsNotCorrect_Itshould_SaveTheRawSms_AndSendSmsBack_WithErrorMessage()
//        {
//            //Arrange
//            objectMother.manageReceivedSmsService.Expect(call => call.GetRawSmsReceivedFromXMLString(objectMother.XMLStringFromDrugShop)).Return(objectMother.rawSmsIncorectFormatDrugShop);
//            objectMother.manageReceivedSmsService.Expect(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(objectMother.rawSmsIncorectFormatDrugShop)).Return(objectMother.rawSmsIncorectFormatDrugShop);
//            objectMother.manageReceivedSmsService.Expect(call => call.ParseRawSmsReceivedFromDrugShop(objectMother.rawSmsIncorectFormatDrugShop)).Return(objectMother.rawSmsIncorectFormatDrugShop);
//            objectMother.smsRequestService.Expect(call => call.SendMessage(Arg<string>.Is.Anything, Arg<RawSmsReceived>.Is.Anything)).Return(true);
//            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
//                p => p.OutpostId == objectMother.outpostId &&
//                     p.Sender == ObjectMother.CORRECTPHONENUMBER &&
//                     p.ParseSucceeded == false
//                     )));
//            objectMother.MockRequest(objectMother.XMLStringFromDrugShop);

//            //Act
//            var result = objectMother.controller.ReceiveSms();

//            //Assert
//            objectMother.manageReceivedSmsService.VerifyAllExpectations();
//            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
//            objectMother.smsRequestService.VerifyAllExpectations();
//            var res = result as EmptyResult;
//            Assert.IsNotNull(res);
//        }

//        [Test]
//        public void WhenMessageIsFromDispensary_ButContentIsNotCorrect_Itshould_SaveTheRawSms_AndSendSmsBack_WithErrorMessage()
//        {
//            //Arrange
//            objectMother.manageReceivedSmsService.Expect(call => call.GetRawSmsReceivedFromXMLString(objectMother.XMLStringDispensary)).Return(objectMother.rawSmsIncorectFormatDispensary);
//            objectMother.manageReceivedSmsService.Expect(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(objectMother.rawSmsIncorectFormatDispensary)).Return(objectMother.rawSmsIncorectFormatDispensary);
//            objectMother.manageReceivedSmsService.Expect(call => call.ParseRawSmsReceivedFromDispensary(objectMother.rawSmsIncorectFormatDispensary)).Return(objectMother.rawSmsIncorectFormatDispensary);
//            objectMother.smsRequestService.Expect(call => call.SendMessage(Arg<string>.Is.Anything, Arg<RawSmsReceived>.Is.Anything)).Return(true);
//            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
//                p => p.OutpostId == objectMother.outpostId &&
//                     p.Sender == ObjectMother.CORRECTPHONENUMBER &&
//                     p.ParseSucceeded == false
//                     )));
//            objectMother.MockRequest(objectMother.XMLStringDispensary);

//            //Act
//            var result = objectMother.controller.ReceiveSms();

//            //Assert
//            objectMother.manageReceivedSmsService.VerifyAllExpectations();
//            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
//            objectMother.smsRequestService.VerifyAllExpectations();
//            var res = result as EmptyResult;
//            Assert.IsNotNull(res);
//        }

//        [Test]
//        public void WhenMessageIsFromDrugShop_ContentIsCorrect_Itshould_SaveTheRawSms_AndSendSmsToDispensary()
//        {
//            //Arrange
//            objectMother.manageReceivedSmsService.Expect(call => call.GetRawSmsReceivedFromXMLString(objectMother.XMLCorrectStringFromDrugShop)).Return(objectMother.rawSmsCorerctFormatDrugShop);
//            objectMother.manageReceivedSmsService.Expect(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(objectMother.rawSmsCorerctFormatDrugShop)).Return(objectMother.rawSmsCorerctFormatDrugShop);
//            objectMother.manageReceivedSmsService.Expect(call => call.ParseRawSmsReceivedFromDrugShop(objectMother.rawSmsCorerctFormatDrugShop)).Return(objectMother.rawSmsCorerctFormatDrugShop);
//            objectMother.manageReceivedSmsService.Expect(call => call.CreateMessageFromDrugShop(objectMother.rawSmsCorerctFormatDrugShop)).Return(objectMother.messageFromDrugShop);
//            objectMother.smsRequestService.Expect(call => call.SendMessageToDispensary(Arg<MessageFromDrugShop>.Is.Anything, Arg<RawSmsReceived>.Is.Anything)).Return(true);

//            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
//                p => p.OutpostId == objectMother.outpostId &&
//                     p.Sender == ObjectMother.CORRECTPHONENUMBER &&
//                     p.ParseSucceeded == true
//                     )));

//            objectMother.saveCommandMessageFromDrugShop.Expect(call => call.Execute(Arg<MessageFromDrugShop>.Matches(
//                p => p.OutpostId == objectMother.outpostId &&
//                     p.Gender == "M" &&
//                     p.IDCode == "12345678"
//                     )));

//            objectMother.MockRequest(objectMother.XMLCorrectStringFromDrugShop);
            
//            //Act
//            var result = objectMother.controller.ReceiveSms();

//            //Assert
//            objectMother.manageReceivedSmsService.VerifyAllExpectations();
//            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
//            objectMother.saveCommandMessageFromDrugShop.VerifyAllExpectations();
//            objectMother.smsRequestService.VerifyAllExpectations();
//            var res = result as EmptyResult;
//            Assert.IsNotNull(res);
//        }

//        [Test]
//        public void WhenMessageIsFromDispensary_AndContentIsCorrent_Itshould_SaveTheRawSms()
//        {
//            //Arrange
//            objectMother.manageReceivedSmsService.Expect(call => call.GetRawSmsReceivedFromXMLString(objectMother.XMLCorrectStringDispensary)).Return(objectMother.rawSmsCorerctFormatDispensary);
//            objectMother.manageReceivedSmsService.Expect(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(objectMother.rawSmsCorerctFormatDispensary)).Return(objectMother.rawSmsCorerctFormatDispensary);
//            objectMother.manageReceivedSmsService.Expect(call => call.ParseRawSmsReceivedFromDispensary(objectMother.rawSmsCorerctFormatDispensary)).Return(objectMother.rawSmsCorerctFormatDispensary);
//            objectMother.manageReceivedSmsService.Expect(call => call.CreateMessageFromDispensary(objectMother.rawSmsCorerctFormatDispensary)).Return(objectMother.messageFromDispensary);

//            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
//                p => p.OutpostId == objectMother.outpostId &&
//                     p.Sender == ObjectMother.CORRECTPHONENUMBER &&
//                     p.ParseSucceeded == true
//                     )));

//            objectMother.saveCommandMessageFromDispensary.Expect(call => call.Execute(Arg<MessageFromDispensary>.Matches(
//                p => p.OutpostId == objectMother.outpostId &&
//                     p.MessageFromDrugShop.Id == objectMother.messageFromDrugShop.Id &&
//                     p.OutpostType == 1
//                     )));
//            objectMother.MockRequest(objectMother.XMLCorrectStringDispensary);

//            //Act
//            var result = objectMother.controller.ReceiveSms();

//            //Assert
//            objectMother.manageReceivedSmsService.VerifyAllExpectations();
//            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
//            objectMother.saveCommandMessageFromDispensary.VerifyAllExpectations();
//            var res = result as EmptyResult;
//            Assert.IsNotNull(res);
//        }


//    }
//}
