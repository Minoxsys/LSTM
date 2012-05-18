using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Areas.ServicesManagement.Models.ServiceNeeded;

namespace Tests.Unit.Controllers.Areas.ServicesManagement.ServiceNeededControllerTests
{
    [TestFixture]
    public class GetServiceNeededMethod
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
            var indexModel = new ServiceNeededIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Code"
            };
            var pageOfData = objectMother.PageOfServiceNeededData(indexModel);
            objectMother.queryServiceNeeded.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetServiceNeeded(indexModel);

            //Assert
            objectMother.queryServiceNeeded.VerifyAllExpectations();

            Assert.IsInstanceOf<ServiceNeededIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as ServiceNeededIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_ServiceNeeded_OrderBy_Keyword_DESC()
        {
            //Arrange
            var indexModel = new ServiceNeededIndexModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Keyword"
            };

            var pageOfData = objectMother.PageOfServiceNeededData(indexModel);
            objectMother.queryServiceNeeded.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetServiceNeeded(indexModel);

            //Assert
            objectMother.queryServiceNeeded.VerifyAllExpectations();

            var jsonData = jsonResult.Data as ServiceNeededIndexOuputModel;
            Assert.That(jsonData.ServiceNeeded[0].Keyword, Is.EqualTo(objectMother.serviceNeeded.Keyword+ "9"));
        }

        [Test]
        public void Returns_ServiceNeeded_ThatContain_SearchValue()
        {
            //Arrange
            var indexModel = new ServiceNeededIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Code",
                searchValue = "9"
            };

            var pageOfData = objectMother.PageOfServiceNeededData(indexModel);
            objectMother.queryServiceNeeded.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetServiceNeeded(indexModel);

            //Assert
            objectMother.queryServiceNeeded.VerifyAllExpectations();

            Assert.IsInstanceOf<ServiceNeededIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as ServiceNeededIndexOuputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(5, jsonData.TotalItems);
        }
    }
}
