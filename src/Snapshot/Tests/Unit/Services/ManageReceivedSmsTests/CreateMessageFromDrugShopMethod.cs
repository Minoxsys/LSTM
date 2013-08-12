using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Services.ManageReceivedSmsTests
{
    [TestFixture]
    public class CreateMessageFromDrugShopMethod
    {
        public ObjectMother ObjectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            ObjectMother.Init();
        }

        [Test]
        public void WhenWeWantToCreateMessageFormDrugShop_ItShouldParseTheRawSmSAndReturnTheCorrectMessageFromSrugShop()
        {
            //Arange
            ObjectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(new[] {ObjectMother.messageFromDrugShop}.AsQueryable());
            var services = ObjectMother.ListOfCondition();
            ObjectMother.queryCondition.Expect(call => call.Query()).Return(services);
            var appointment = ObjectMother.ListOfAppointment();
            ObjectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = ObjectMother.service.CreateMessageFromDrugShop(ObjectMother.rawSmsReceived);

            //Assert
            ObjectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            ObjectMother.queryCondition.VerifyAllExpectations();
            ObjectMother.queryAppointment.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageFromDrugShop>(result);
            Assert.AreEqual("F", result.Gender);
            Assert.AreEqual("XYXX", result.Initials);
            Assert.AreEqual("S1", result.ServicesNeeded[0].Code);
            Assert.AreEqual("H1", result.Appointments[0].Code);
        }

        [Test]
        public void WhenWeWantToCreateMessageFormDrugShop_WhenMessageContainsPhoneNUmber_ItShouldParseTheRawSmSAndReturnTheCorrectMessageFromSrugShop()
        {
            //Arange
            ObjectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(new[] { ObjectMother.messageFromDrugShop }.AsQueryable());
            var services = ObjectMother.ListOfCondition();
            ObjectMother.queryCondition.Expect(call => call.Query()).Return(services);
            var appointment = ObjectMother.ListOfAppointment();
            ObjectMother.queryAppointment.Expect(call => call.Query()).Return(appointment);

            //Act
            var result = ObjectMother.service.CreateMessageFromDrugShop(ObjectMother.rawSmsReceivedWithPhoneNumber);

            //Assert
            ObjectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            ObjectMother.queryCondition.VerifyAllExpectations();
            ObjectMother.queryAppointment.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageFromDrugShop>(result);
            Assert.AreEqual("F", result.Gender);
            Assert.AreEqual("XYXX", result.Initials);
            Assert.AreEqual("S1", result.ServicesNeeded[0].Code);
            Assert.AreEqual("H1", result.Appointments[0].Code);
            Assert.AreEqual("+255123456789", result.PatientPhoneNumber);
        }
    }
}
