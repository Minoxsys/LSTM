using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Services.ManageReceivedSmsTests
{
    [TestFixture]
    public class ParseRawSmsReceivedFromDrugShopMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void WhenWeWantToParseTheSms_CorrectContentWithInvalidPhoneNumberd_ReturnsErrroMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.CorrectContentWithInvalidPhoneNumber };
            var services = objectMother.ListOfCondition();
            objectMother.queryCondition.Expect(call => call.Query()).Return(services);
            var appointment = objectMother.ListOfAppointment();
            objectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(false, result.ParseSucceeded);
            Assert.AreEqual("The patient's phone number has incorrect format.", result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_CorrectContentWithInvalidPhoneNumberNoPlusSign_ReturnsErrroMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.CorrectContentWithInvalidPhoneNumberNoPlusSign };
            var services = objectMother.ListOfCondition();
            objectMother.queryCondition.Expect(call => call.Query()).Return(services);
            var appointment = objectMother.ListOfAppointment();
            objectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(false, result.ParseSucceeded);
            Assert.AreEqual("The patient's phone number has incorrect format.", result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_CorrectContentWithValidPhoneNumberPlusSignBeforeCountryCode_ReturnsNoErrroMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.CorrectContentWithValidPhoneNumberPlusSignBeforeCountryCode };
            var services = objectMother.ListOfCondition();
            objectMother.queryCondition.Expect(call => call.Query()).Return(services);
            var appointment = objectMother.ListOfAppointment();
            objectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(true, result.ParseSucceeded);
            Assert.IsNullOrEmpty(result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_CorrectContentWithValidPhoneNumberDoubleZeroBeforeCountryCode_ReturnsNoErrroMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.CorrectContentWithValidPhoneNumberDoubleZeroBeforeCountryCode };
            var services = objectMother.ListOfCondition();
            objectMother.queryCondition.Expect(call => call.Query()).Return(services);
            var appointment = objectMother.ListOfAppointment();
            objectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(true, result.ParseSucceeded);
            Assert.IsNullOrEmpty(result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_CorrectContentWithValidPhoneNumberNothingBeforeCountryCode_ReturnsNoErrroMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.CorrectContentWithValidPhoneNumberNothingBeforeCountryCode };
            var services = objectMother.ListOfCondition();
            objectMother.queryCondition.Expect(call => call.Query()).Return(services);
            var appointment = objectMother.ListOfAppointment();
            objectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(true, result.ParseSucceeded);
            Assert.IsNullOrEmpty(result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_CorrectContentWithValidPhoneNumberWithoutCountryCode_ReturnsNoErrroMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.CorrectContentWithValidPhoneNumberWithoutCountryCode};
            var services = objectMother.ListOfCondition();
            objectMother.queryCondition.Expect(call => call.Query()).Return(services);
            var appointment = objectMother.ListOfAppointment();
            objectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(true, result.ParseSucceeded);
            Assert.IsNullOrEmpty(result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_CorrectContentWithInvalidPhoneNumberPlusSignBeforeCountryCodeButWrongCountryCode_ReturnsErrroMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.CorrectContentWithInvalidPhoneNumberPlusSignBeforeCountryCodeButWrongCountryCode };
            var services = objectMother.ListOfCondition();
            objectMother.queryCondition.Expect(call => call.Query()).Return(services);
            var appointment = objectMother.ListOfAppointment();
            objectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(false, result.ParseSucceeded);
            Assert.AreEqual("The patient's phone number has incorrect format.", result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_CorrectContentWithInvalidPhoneNumberDoubleZeroBeforeCountryCodeButWrongCountryCode_ReturnsErrroMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.CorrectContentWithInvalidPhoneNumberDoubleZeroBeforeCountryCodeButWrongCountryCode };
            var services = objectMother.ListOfCondition();
            objectMother.queryCondition.Expect(call => call.Query()).Return(services);
            var appointment = objectMother.ListOfAppointment();
            objectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(false, result.ParseSucceeded);
            Assert.AreEqual("The patient's phone number has incorrect format.", result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_CorrectContentWithInvalidPhoneNumberNothingBeforeCountryCodeButWrongCountryCode_ReturnsErrroMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.CorrectContentWithInvalidPhoneNumberNothingBeforeCountryCodeButWrongCountryCode };
            var services = objectMother.ListOfCondition();
            objectMother.queryCondition.Expect(call => call.Query()).Return(services);
            var appointment = objectMother.ListOfAppointment();
            objectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(false, result.ParseSucceeded);
            Assert.AreEqual("The patient's phone number has incorrect format.", result.ParseErrorMessage);
        }


        [Test]
        public void WhenWeWantToParseTheSms_AndTheContentIsNotInTheRightFormat_ItShouldReturnErrorMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.WRONGCONTENTDRUGSHOP };

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(false, result.ParseSucceeded);
            Assert.AreEqual("The format of the message is incorrect.", result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_AndContentIsCorrect_ButTheDateIsNotValid_ItShouldReturnErrorMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.WRONGDATEMESSAGEFROMDRUGSHOP };

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(false, result.ParseSucceeded);
            Assert.AreEqual("Date 231398 is incorect.", result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_AndContentIsCorrect_ButServiceCodeIsIncorect_ItShouldReturnErrorMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.WRONGSERVICEMESSAGEFROMDRUGSHOP };
            var services = objectMother.ListOfCondition();
            objectMother.queryCondition.Expect(call => call.Query()).Return(services);
            var appointment = objectMother.ListOfAppointment();
            objectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(false, result.ParseSucceeded);
            Assert.AreEqual("Service RX1 is incorect.", result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_AndContentIsCorrect_ServicesExists_ThereShouldBeNOEroorMessages()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.CORRECTMESSAGEFROMDRUGSHOP };
            var services = objectMother.ListOfCondition();
            var appointment = objectMother.ListOfAppointment();

            objectMother.queryCondition.Expect(call => call.Query()).Return(services);
            objectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(true, result.ParseSucceeded);
            Assert.AreEqual(result.ParseErrorMessage, null);
        }
    }
}
