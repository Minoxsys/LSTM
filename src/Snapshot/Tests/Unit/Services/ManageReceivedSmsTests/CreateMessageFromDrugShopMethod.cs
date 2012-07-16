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
        public void WhenWeWantToCreateMessageFormDrugShop_ItShouldParseTheRawSmSAndReturnTheCorrectMessageFromSrugShop()
        {
            //Arange
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(new MessageFromDrugShop[] { objectMother.messageFromDrugShop }.AsQueryable());
            var services = objectMother.ListOfCondition();
            objectMother.queryCondition.Expect(call => call.Query()).Return(services);
            var appointment = objectMother.ListOfAppointment();
            objectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = objectMother.service.CreateMessageFromDrugShop(objectMother.rawSmsReceived);

            //Assert
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            objectMother.queryCondition.VerifyAllExpectations();
            objectMother.queryAppointment.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageFromDrugShop>(result);
            Assert.AreEqual("F", result.Gender);
            Assert.AreEqual("XYXX", result.Initials);
            Assert.AreEqual("S1", result.ServicesNeeded[0].Code);
            Assert.AreEqual("H1", result.Appointments[0].Code);
        }
    }
}
