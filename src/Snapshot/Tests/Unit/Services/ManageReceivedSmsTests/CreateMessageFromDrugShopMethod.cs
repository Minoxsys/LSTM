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
    public class CreateMessageFromDrugShopMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void ItShouldCreateAMessageFromDrugShop_FromAGivenRawSmsReceived()
        {
            //Arrange
            var services = objectMother.ListOfServiceNeeded();
            var messages = objectMother.ListOfMessagesFromDrugSho();
            objectMother.queryServiceNeeded.Expect(call => call.Query()).Return(services);
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(messages);

            //Act
            var result = objectMother.service.CreateMessageFromDrugShop(objectMother.rawSmsReceived);

            //Assert
            objectMother.queryServiceNeeded.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageFromDrugShop>(result);
            Assert.AreEqual("XYXX", result.Initials);
            Assert.AreEqual(new DateTime(1998,4,23), result.BirthDate);
            Assert.AreEqual("F", result.Gender);
            Assert.AreEqual(3, result.ServicesNeeded.Count());
        }
    }
}
