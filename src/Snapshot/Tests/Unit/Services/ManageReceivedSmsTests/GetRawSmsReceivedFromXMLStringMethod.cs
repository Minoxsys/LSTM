using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Services.ManageReceivedSmsTests
{
    public class GetRawSmsReceivedFromXMLStringMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void WhenWeWantToParseAnXMLString_AndXMLIsCorrect_ItShouldReturnRawSmsWithContent()
        {
            //Arrange

            //Act
            var result = objectMother.service.GetRawSmsReceivedFromXMLString(ObjectMother.CORRECTXML);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual("54321", result.SmsId);
            Assert.AreEqual(DateTime.Parse("2008-10-13 13:30:10"), result.ReceivedDate);
            Assert.AreEqual("+79991234567", result.Sender);
            Assert.AreEqual("1234", result.ServiceNumber);
            Assert.AreEqual("operator-smpp", result.Operator);
            Assert.AreEqual("100", result.OperatorId);
            Assert.AreEqual("This", result.Keyword);
            Assert.AreEqual("This is a test message", result.Content);
        }

        [Test]
        public void WhenWeWantToParseAnXMLString_AndXMLIsCorrect_ItShouldReturnNewRawSms()
        {
            //Arrange

            //Act
            var result = objectMother.service.GetRawSmsReceivedFromXMLString(ObjectMother.WRONGXML);

            //Assert
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.IsNull(result.SmsId);
            Assert.IsNull(result.Sender);
            Assert.IsNull(result.ServiceNumber);
            Assert.IsNull(result.Operator);
            Assert.IsNull(result.OperatorId);
            Assert.IsNull(result.Keyword);
            Assert.IsNull(result.Content);
        }
    }
}
