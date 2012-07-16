using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.ConditionManagement.Models.Appointment;
using Web.Models.Shared;
using Domain;

namespace Tests.Unit.Controllers.Areas.ConditionManagement.AppointmentControllerTests
{
    [TestFixture]
    public class CreateMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_Appointment_Code_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Create(new AppointmentModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("The appointment has not been saved!"));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_Appointment_Code_AND_Keyword_AlreadyExists()
        {
            //Arrange
            AppointmentModel appointmentInputModel = new AppointmentModel()
            {
                Keyword = objectMother.appointment.Keyword,
                Code = objectMother.appointment.Code,
                Description = "new description"
            };
            objectMother.queryAppointment.Expect(call => call.Query()).Return(new Appointment[] { objectMother.appointment }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.Create(appointmentInputModel);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Appointment_Has_Been_Saved()
        {
            //Arrange
            AppointmentModel appointmentInputModel = new AppointmentModel()
            {
                Keyword = "Family planning",
                Code = "FP-8",
                Description = "Tubal ligation"
            };
            objectMother.queryAppointment.Expect(call => call.Query()).Return(new Appointment[] { objectMother.appointment }.AsQueryable());
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Appointment>.Matches(p => p.Code == appointmentInputModel.Code)));

            //Act
            var jsonResult = objectMother.controller.Create(appointmentInputModel);

            //Assert
            objectMother.queryAppointment.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual("Success", response.Status);
            Assert.AreEqual("Appointment " + appointmentInputModel.Code + " has been saved.", response.Message);
        }
    }
}
