using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.AnalysisManagement.Models.PatientsReport;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.PatientsReportControllerTests
{
    [TestFixture]
    public class GetAdviceMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_List_Of_Advices_For_User()
        {
            //Arange
            objectMother.queryAdvice.Expect(call => call.Query()).Return(new Advice[] { objectMother.advice }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetAdvice();

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<ServiceIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as ServiceIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }
    }
}
