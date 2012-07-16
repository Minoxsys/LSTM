using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.ConditionManagement.Models.Appointment;

namespace Tests.Unit.Controllers.Areas.ConditionManagement.AppointmentControllerTests
{
    [TestFixture]
    public class GetAppointmentMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_The_Data_Paginated_BasedOnTheInputValues()
        {
            //Arrange
            var indexModel = new AppointmentIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Code"
            };
            var pageOfData = objectMother.PageOfAppointmentData(indexModel);
            objectMother.queryAppointment.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetAppointment(indexModel);

            //Assert
            objectMother.queryAppointment.VerifyAllExpectations();

            Assert.IsInstanceOf<AppointmentIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as AppointmentIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_Appointment_OrderBy_Keyword_DESC()
        {
            //Arrange
            var indexModel = new AppointmentIndexModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Keyword"
            };

            var pageOfData = objectMother.PageOfAppointmentData(indexModel);
            objectMother.queryAppointment.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetAppointment(indexModel);

            //Assert
            objectMother.queryAppointment.VerifyAllExpectations();

            var jsonData = jsonResult.Data as AppointmentIndexOuputModel;
            Assert.That(jsonData.Appointment[0].Keyword, Is.EqualTo(objectMother.appointment.Keyword + "9"));
        }

        [Test]
        public void Returns_Appointment_ThatContain_SearchValue()
        {
            //Arrange
            var indexModel = new AppointmentIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Code",
                searchValue = "9"
            };

            var pageOfData = objectMother.PageOfAppointmentData(indexModel);
            objectMother.queryAppointment.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetAppointment(indexModel);

            //Assert
            objectMother.queryAppointment.VerifyAllExpectations();

            Assert.IsInstanceOf<AppointmentIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as AppointmentIndexOuputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(5, jsonData.TotalItems);
        }
    }
}
