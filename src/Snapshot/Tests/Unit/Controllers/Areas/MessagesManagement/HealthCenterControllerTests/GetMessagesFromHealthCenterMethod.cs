using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.MessagesManagement.Models.Messages;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.MessagesManagement.HealthCenterControllerTests
{
    [TestFixture]
    public class GetMessagesFromHealthCenterMethod
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
            var indexModel = new MessagesIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Sender"
            };
            var pageOfData = objectMother.PageOfData(indexModel);
            objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetMessagesFromHealthCenter(indexModel);

            //Assert
            objectMother.queryRawSms.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MessageIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_OnlyMessagesFromHealthCenters()
        {
            //Arrange
            var indexModel = new MessagesIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Sender"
            };
            var pageOfData = objectMother.PageOfDrugShopData(indexModel);
            objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetMessagesFromHealthCenter(indexModel);

            //Assert
            objectMother.queryRawSms.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MessageIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count() / 3, jsonData.TotalItems);
        }

        [Test]
        public void Returns_OnlyMessagesFromHealthCenter_Order_DESC_by_Content()
        {
            //Arrange
            var indexModel = new MessagesIndexModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Content"
            };
            var pageOfData = objectMother.PageOfDrugShopData(indexModel);
            objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetMessagesFromHealthCenter(indexModel);

            //Assert
            objectMother.queryRawSms.VerifyAllExpectations();

            var jsonData = jsonResult.Data as MessageIndexOuputModel;
            Assert.That(jsonData.Messages[0].Content, Is.EqualTo(objectMother.rawSms.Content + "-8"));
        }
        [Test]
        public void Returns_OnlyMessagesFromHealthCenter_WhereContentContains_SearchValue()
        {
            //Arrange
            var indexModel = new MessagesIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Sender",
                searchValue = "-8"
            };

            var pageOfData = objectMother.PageOfDrugShopData(indexModel);
            objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetMessagesFromHealthCenter(indexModel);

            //Assert
            objectMother.queryRawSms.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MessageIndexOuputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(1, jsonData.TotalItems);
        }
    }
}
