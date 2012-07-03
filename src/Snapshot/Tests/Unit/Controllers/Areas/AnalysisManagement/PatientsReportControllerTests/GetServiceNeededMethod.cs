using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Areas.AnalysisManagement.Models.PatientsReport;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.PatientsReportControllerTests
{
    [TestFixture]
    public class GetConditionMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_List_Of_Service_Needed_For_User()
        {
            //Arange
            objectMother.queryCondition.Expect(call => call.Query()).Return(new Condition[] { objectMother.condition }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetCondition();

            //Assert
            objectMother.queryCondition.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<ServiceIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as ServiceIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }
    }
}
