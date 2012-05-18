using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.ServicesManagement.Models.Diagnosis;

namespace Tests.Unit.Controllers.Areas.ServicesManagement.DiagnosisControllerTests
{
    [TestFixture]
    public class GetDiagnosisMethod
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
            var indexModel = new DiagnosisIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Code"
            };
            var pageOfData = objectMother.PageOfDiagnosisData(indexModel);
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetDiagnosis(indexModel);

            //Assert
            objectMother.queryDiagnosis.VerifyAllExpectations();

            Assert.IsInstanceOf<DiagnosisIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as DiagnosisIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_Diagnosis_OrderBy_Keyword_DESC()
        {
            //Arrange
            var indexModel = new DiagnosisIndexModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Keyword"
            };

            var pageOfData = objectMother.PageOfDiagnosisData(indexModel);
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetDiagnosis(indexModel);

            //Assert
            objectMother.queryDiagnosis.VerifyAllExpectations();

            var jsonData = jsonResult.Data as DiagnosisIndexOuputModel;
            Assert.That(jsonData.Diagnosis[0].Keyword, Is.EqualTo("Chlamydia9"));
        }

        [Test]
        public void Returns_Diagnosis_ThatContain_SearchValue()
        {
            //Arrange
            var indexModel = new DiagnosisIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Code",
                searchValue = "9"
            };

            var pageOfData = objectMother.PageOfDiagnosisData(indexModel);
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetDiagnosis(indexModel);

            //Assert
            objectMother.queryDiagnosis.VerifyAllExpectations();

            Assert.IsInstanceOf<DiagnosisIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as DiagnosisIndexOuputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(5, jsonData.TotalItems);
        }
    }
}
