using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.ServicesManagement.Models.Treatment;

namespace Tests.Unit.Controllers.Areas.ServicesManagement.TreatmentControllerTests
{
    [TestFixture]
    public class GetTreatmentsMethod
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
            var indexModel = new TreatmentIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Code"
            };
            var pageOfData = objectMother.PageOfTreatmentData(indexModel);
            objectMother.queryTreatment.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetTreatments(indexModel);

            //Assert
            objectMother.queryTreatment.VerifyAllExpectations();

            Assert.IsInstanceOf<TreatmentIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as TreatmentIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_Treatment_OrderBy_Advice_DESC()
        {
            //Arrange
            var indexModel = new TreatmentIndexModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Advice"
            };

            var pageOfData = objectMother.PageOfTreatmentData(indexModel);
            objectMother.queryTreatment.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetTreatments(indexModel);

            //Assert
            objectMother.queryTreatment.VerifyAllExpectations();

            var jsonData = jsonResult.Data as TreatmentIndexOuputModel;
            Assert.That(jsonData.Treatments[0].Advice, Is.EqualTo("Advice9"));
        }

        [Test]
        public void Returns_Treatment_ThatContain_SearchValue()
        {
            //Arrange
            var indexModel = new TreatmentIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Code",
                searchValue = "9"
            };

            var pageOfData = objectMother.PageOfTreatmentData(indexModel);
            objectMother.queryTreatment.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetTreatments(indexModel);

            //Assert
            objectMother.queryTreatment.VerifyAllExpectations();

            Assert.IsInstanceOf<TreatmentIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as TreatmentIndexOuputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(5, jsonData.TotalItems);
        }
    }
}
