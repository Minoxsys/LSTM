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
    public class EditMethod
    {
        public ObjectMother objectMother = new ObjectMother();

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
            var jsonResult = objectMother.controller.Edit(new AppointmentModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a appointmentId in order to edit the appointment."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_ThereIsAllreadyAAppointment_WithTheSame_Code_and_Keyword()
        {
            //Arrange
            AppointmentModel appointmentModel = new AppointmentModel()
            {
                Id = Guid.NewGuid(),
                Code = objectMother.appointment.Code,
                Keyword = objectMother.appointment.Keyword,
                Description = objectMother.appointment.Description
            };
            objectMother.queryAppointment.Expect(call => call.Query()).Return(new Appointment[] { objectMother.appointment }.AsQueryable());
            //Act
            var jsonResult = objectMother.controller.Edit(appointmentModel);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }
        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Appointment_HasBeenSaved()
        {
            //Arrange
            AppointmentModel appointmentModel = new AppointmentModel()
            {
                Id = objectMother.appointment.Id,
                Code = "new Code",
                Keyword = "new keyword",
                Description = "new description"
            };
            objectMother.queryAppointment.Expect(call => call.Query()).Return(new Appointment[] { objectMother.appointment }.AsQueryable());
            objectMother.queryAppointment.Expect(call => call.Load(objectMother.appointment.Id)).Return(new Appointment{Messages= objectMother.appointment.Messages});
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Appointment>.Matches(p => p.Code != objectMother.appointment.Code)));

            //Act
            var jsonResult = objectMother.controller.Edit(appointmentModel);

            //Assert
            objectMother.queryAppointment.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Appointment " + appointmentModel.Code + " has been saved."));
        }
    }
}
