using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Models.Shared;
using Domain;

namespace Tests.Unit.Controllers.Areas.ConditionManagement.AppointmentControllerTests
{
    [TestFixture]
    public class DeleteMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_AppointmentId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Delete(null);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a appointmentId in order to remove the appointment."));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Appointment_HasBeenDeleted()
        {
            //Arrange
            objectMother.queryAppointment.Expect(call => call.Load(objectMother.appointmentId)).Return(objectMother.appointment);
            objectMother.deleteCommand.Expect(call => call.Execute(Arg<Appointment>.Matches(p => p.Id == objectMother.appointmentId)));

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.appointment.Id);

            //Assert
            objectMother.queryAppointment.VerifyAllExpectations();
            objectMother.deleteCommand.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            var response = jsonResult.Data as JsonActionResponse;
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Appointment with code " + objectMother.appointment.Code + " was removed."));
        }
    }
}
