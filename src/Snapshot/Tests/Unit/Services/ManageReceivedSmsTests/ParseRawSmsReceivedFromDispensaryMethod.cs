using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using Rhino.Mocks;

namespace Tests.Unit.Services.ManageReceivedSmsTests
{
    [TestFixture]
    public class ParseRawSmsReceivedFromDispensaryMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void WhenWeWantToParseTheSms_AndTheContentIsNotInTheRightFormat_ItShouldReturnErrorMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.WRONGCONTENTDRUGSHOP };
            
            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDispensary(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(false, result.ParseSucceeded);
            Assert.AreEqual("The format of your message is incorrect. Please check and retry. Thank you.", result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_AndContentIsCorrect_ButIDCodeDoesNotExist_ItShouldReturnErrorMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.WRONGIDCODEMESSAGEDISPENSARY };
            var messages = objectMother.ListOfMessagesFromDrugSho();
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(messages);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDispensary(smsReceived);

            //Assert
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(false, result.ParseSucceeded);
            Assert.AreEqual("ID code 12343214 is incorect. Please check and retry. Thank you.", result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_AndContentIsCorrect_ButServiceCodeIsNotDiagnosisOrTreatmentOrAdvice_ItShouldReturnErrorMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.WRONGSERVICECODEFORDISPENSARYMESSAGE };
            var diagnosis = objectMother.ListOfDiagnosis();
            var treatments = objectMother.ListOfTreatments();
            var advices = objectMother.ListOfAdvices();
            var messages = objectMother.ListOfMessagesFromDrugSho();
            objectMother.queryAdvice.Expect(call => call.Query()).Return(advices);
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(diagnosis);
            objectMother.queryTreatment.Expect(call => call.Query()).Return(treatments);
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(messages);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDispensary(smsReceived);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.queryTreatment.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(false, result.ParseSucceeded);
            Assert.AreEqual("Service DIAG1 is incorect. Please check and retry. Thank you.", result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_AndContentIsCorrect_ItShouldReturnNOErrorMessages()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.CORRECTMESSAGEFROMDISPENSARY };
            var diagnosis = objectMother.ListOfDiagnosis();
            var treatments = objectMother.ListOfTreatments();
            var advices = objectMother.ListOfAdvices();
            var messages = objectMother.ListOfMessagesFromDrugSho();
            objectMother.queryAdvice.Expect(call => call.Query()).Return(advices);
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(diagnosis);
            objectMother.queryTreatment.Expect(call => call.Query()).Return(treatments);
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(messages);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDispensary(smsReceived);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.queryTreatment.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(true, result.ParseSucceeded);
            Assert.AreEqual(null, result.ParseErrorMessage);
        }
    
    }
}
