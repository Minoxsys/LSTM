using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.MessagesManagement.Models;
using Rhino.Mocks;
using Web.Areas.MessagesManagement.Models.Drugstore;

namespace Tests.Unit.Controllers.Areas.MessagesManagement.DrugstoreControllerTests
{
    [TestFixture]
    public class GetMessagesFromDrugstoreMethod
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
            var jsonResult = objectMother.controller.GetMessagesFromDrugstore(indexModel);

            //Assert
            objectMother.queryRawSms.VerifyAllExpectations();

            Assert.IsInstanceOf<DrugstoreIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as DrugstoreIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_OnlyMessagesFromDrugstores()
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
            var pageOfData = objectMother.PageOfDrugstoreData(indexModel);
            objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetMessagesFromDrugstore(indexModel);

            //Assert
            objectMother.queryRawSms.VerifyAllExpectations();

            Assert.IsInstanceOf<DrugstoreIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as DrugstoreIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count() / 2, jsonData.TotalItems);
        }

        [Test]
        public void Returns_OnlyMessagesFromDrugstores_Order_DESC_by_Content()
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
            var pageOfData = objectMother.PageOfDrugstoreData(indexModel);
            objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetMessagesFromDrugstore(indexModel);

            //Assert
            objectMother.queryRawSms.VerifyAllExpectations();

            var jsonData = jsonResult.Data as DrugstoreIndexOuputModel;
            Assert.That(jsonData.Messages[0].Content, Is.EqualTo(objectMother.rawSms.Content+ "-9"));
        }
        [Test]
        public void Returns_OnlyMessagesFromDrugstores_WhereContentContains_SearchValue()
        {
            //Arrange
            var indexModel = new MessagesIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Sender",
                searchValue = "9"
            };

            var pageOfData = objectMother.PageOfDrugstoreData(indexModel);
            objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetMessagesFromDrugstore(indexModel);

            //Assert
            objectMother.queryRawSms.VerifyAllExpectations();

            Assert.IsInstanceOf<DrugstoreIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as DrugstoreIndexOuputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(5, jsonData.TotalItems);
        }

    }
}
