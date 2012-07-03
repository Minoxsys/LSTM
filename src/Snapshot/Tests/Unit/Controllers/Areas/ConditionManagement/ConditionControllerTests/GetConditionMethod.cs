using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Areas.ConditionManagement.Models.Condition;

namespace Tests.Unit.Controllers.Areas.ConditionManagement.ConditionControllerTests
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
        public void Returns_The_Data_Paginated_BasedOnTheInputValues()
        {
            //Arrange
            var indexModel = new ConditionIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Code"
            };
            var pageOfData = objectMother.PageOfConditionData(indexModel);
            objectMother.queryCondition.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetCondition(indexModel);

            //Assert
            objectMother.queryCondition.VerifyAllExpectations();

            Assert.IsInstanceOf<ConditionIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as ConditionIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_Condition_OrderBy_Keyword_DESC()
        {
            //Arrange
            var indexModel = new ConditionIndexModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Keyword"
            };

            var pageOfData = objectMother.PageOfConditionData(indexModel);
            objectMother.queryCondition.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetCondition(indexModel);

            //Assert
            objectMother.queryCondition.VerifyAllExpectations();

            var jsonData = jsonResult.Data as ConditionIndexOuputModel;
            Assert.That(jsonData.Condition[0].Keyword, Is.EqualTo(objectMother.condition.Keyword+ "9"));
        }

        [Test]
        public void Returns_Condition_ThatContain_SearchValue()
        {
            //Arrange
            var indexModel = new ConditionIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Code",
                searchValue = "9"
            };

            var pageOfData = objectMother.PageOfConditionData(indexModel);
            objectMother.queryCondition.Expect(call => call.Query()).Return(pageOfData);

            //Act

            var jsonResult = objectMother.controller.GetCondition(indexModel);

            //Assert
            objectMother.queryCondition.VerifyAllExpectations();

            Assert.IsInstanceOf<ConditionIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as ConditionIndexOuputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(5, jsonData.TotalItems);
        }
    }
}
