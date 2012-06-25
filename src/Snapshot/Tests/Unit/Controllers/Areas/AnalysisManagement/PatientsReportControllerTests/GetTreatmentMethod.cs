using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using Domain;
using Web.Areas.AnalysisManagement.Models.PatientsReport;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.PatientsReportControllerTests
{
    [TestFixture]
    public class GetTreatmentMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_List_Of_Treatments_For_User()
        {
            //Arange
            objectMother.queryTreatment.Expect(call => call.Query()).Return(new Treatment[] { objectMother.treatment }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetTreatment();

            //Assert
            objectMother.queryTreatment.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<ServiceIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as ServiceIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }
    }
}
