using System;
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
        public void IfNoOutpostHasAssociatedThePhoneNumber_ItShouldSaveTheRawSmsMessage_AndSendAnErrorMessage()
        {
            //Arrange
            objectMother.manageReceivedSmsService.Stub(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(objectMother.rawSms)).Return(Guid.Empty);
            objectMother.smsRequestService.Stub(call => call.SendMessage(Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
                p => p.OutpostId == Guid.Empty && 
                     p.Sender == ObjectMother.WRONGPHONENUMBER && 
                     p.Content == ObjectMother.TEXT &&
                     p.ParseSucceeded == false &&
                     p.ParseErrorMessage == "Phone number is not valid."
                     )));

            //Act
            var result = objectMother.controller.ReceiveSms(ObjectMother.WRONGPHONENUMBER, ObjectMother.SERVICENUMBER, ObjectMother.OPERATOR, ObjectMother.DATE, ObjectMother.TEXT);
            
            //Assert
            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
            Assert.IsNull(result);
        }

        [Test]
        public void IfPhoneNumberIdCorrect_AndOutpostIdDrugShop_ItShouldParseTheTextMessage_IfTextMessageIsNotCorrect_ItShouldSendErrorMessageAndReturnNull()
        {
            //Arange
            objectMother.manageReceivedSmsService.Stub(call => call.AssignOutpostToRawSmsReceivedBySenderNumber(Arg<RawSmsReceived>.Is.Anything)).Return(objectMother.outpostId);
            objectMother.smsRequestService.Stub(call => call.SendMessage(Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(true);
            objectMother.manageReceivedSmsService.Stub(call => call.ParseRawSmsReceivedFromDrugShop(Arg<RawSmsReceived>.Is.Anything)).Return(objectMother.rawSms);

            objectMother.saveCommandRawSmsReceived.Expect(call => call.Execute(Arg<RawSmsReceived>.Matches(
                p => p.OutpostId == Guid.Empty &&
                     p.Sender == ObjectMother.CORRECTPHONENUMBER &&
                     p.Content == ObjectMother.TEXT &&
                     p.ParseSucceeded == false &&
                     p.ParseErrorMessage == "Phone number is not valid."
                     )));

            //Act
            var result = objectMother.controller.ReceiveSms(ObjectMother.CORRECTPHONENUMBER, ObjectMother.SERVICENUMBER, ObjectMother.OPERATOR, ObjectMother.DATE, ObjectMother.TEXT);

            //Assert
            objectMother.saveCommandRawSmsReceived.VerifyAllExpectations();
            Assert.IsNull(result);
        }
        
    }
}
