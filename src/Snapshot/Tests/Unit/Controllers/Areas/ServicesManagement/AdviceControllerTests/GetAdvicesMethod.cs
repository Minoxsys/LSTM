using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.ServicesManagement.Models.Advice;

namespace Tests.Unit.Controllers.Areas.ServicesManagement.AdviceControllerTests
{
    [TestFixture]
    public class GetAdvicesMethod
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
            var indexModel = new AdviceIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Code"
            };
            var pageOfData = objectMother.PageOfAdviceData(indexModel);
            objectMother.queryAdvice.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetAdvices(indexModel);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();

            Assert.IsInstanceOf<AdviceIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as AdviceIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_Advice_OrderBy_Keyword_DESC()
        {
            //Arrange
            var indexModel = new AdviceIndexModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Keyword"
            };

            var pageOfData = objectMother.PageOfAdviceData(indexModel);
            objectMother.queryAdvice.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetAdvices(indexModel);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();

            var jsonData = jsonResult.Data as AdviceIndexOuputModel;
            Assert.That(jsonData.Advices[0].Keyword, Is.EqualTo(objectMother.advice.Keyword+"9"));
        }

        [Test]
        public void Returns_Advice_ThatContain_SearchValue()
        {
            //Arrange
            var indexModel = new AdviceIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Code",
                searchValue = "9"
            };

            var pageOfData = objectMother.PageOfAdviceData(indexModel);
            objectMother.queryAdvice.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetAdvices(indexModel);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();

            Assert.IsInstanceOf<AdviceIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as AdviceIndexOuputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(5, jsonData.TotalItems);
        }
    }
}
