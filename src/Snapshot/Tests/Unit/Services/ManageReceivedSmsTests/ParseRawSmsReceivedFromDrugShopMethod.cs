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
        public void WhenWeWantToParseTheSms_AndTheContentIsNotInTheRightFormat_ItShouldReturnErrorMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.WRONGCONTENTDRUGSHOP };

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(false, result.ParseSucceeded);
            Assert.AreEqual("The format of your message is incorrect. Please check and retry. Thank you.", result.ParseErrorMessage);
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
            Assert.AreEqual("Date 231398 is incorect. Please check and retry. Thank you.", result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_AndContentIsCorrect_ButServiceCodeIsIncorect_ItShouldReturnErrorMessage()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.WRONGSERVICEMESSAGEFROMDRUGSHOP };
            var services = objectMother.ListOfCondition();
            objectMother.queryCondition.Expect(call => call.Query()).Return(services);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(false, result.ParseSucceeded);
            Assert.AreEqual("Service RX1 is incorect. Please check and retry. Thank you.", result.ParseErrorMessage);
        }

        [Test]
        public void WhenWeWantToParseTheSms_AndContentIsCorrect_ServicesExists_ThereShouldBeNOEroorMessages()
        {
            //Arrange
            RawSmsReceived smsReceived = new RawSmsReceived { Content = ObjectMother.CORRECTMESSAGEFROMDRUGSHOP };
            var services = objectMother.ListOfCondition();
            objectMother.queryCondition.Expect(call => call.Query()).Return(services);

            //Act
            var result = objectMother.service.ParseRawSmsReceivedFromDrugShop(smsReceived);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(true, result.ParseSucceeded);
            Assert.AreEqual(result.ParseErrorMessage, null);
        }
    }
}
