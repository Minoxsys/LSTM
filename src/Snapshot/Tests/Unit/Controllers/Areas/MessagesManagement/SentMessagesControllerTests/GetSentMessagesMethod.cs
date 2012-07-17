using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.MessagesManagement.Models.SentMessages;

namespace Tests.Unit.Controllers.Areas.MessagesManagement.SentMessagesControllerTests
{
    [TestFixture]
    public class GetSentMessagesMethod
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
            var indexModel = new SentMessagesIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "PhoneNumber"
            };
            var pageOfData = objectMother.PageOfData(indexModel);
            objectMother.querySms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetSentMessages(indexModel);

            //Assert
            objectMother.querySms.VerifyAllExpectations();

            Assert.IsInstanceOf<SentMessageIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as SentMessageIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(50, jsonData.TotalItems);
        }

        [Test]
        public void Returns_Messages_Order_DESC_by_ContentMessage()
        {
            //Arrange
            var indexModel = new SentMessagesIndexModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Message"
            };
            var pageOfData = objectMother.PageOfData(indexModel);
            objectMother.querySms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetSentMessages(indexModel);

            //Assert
            objectMother.querySms.VerifyAllExpectations();

            var jsonData = jsonResult.Data as SentMessageIndexOuputModel;
            Assert.That(jsonData.Messages[0].Message, Is.EqualTo(ObjectMother.MESSAGE+"9"));
        }

        [Test]
        public void Returns_Messages_WhereMessageContentContains_SearchValue()
        {
            //Arrange
            var indexModel = new SentMessagesIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "PhoneNumber",
                searchValue = "8"
            };

            var pageOfData = objectMother.PageOfData(indexModel);
            objectMother.querySms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = objectMother.controller.GetSentMessages(indexModel);

            //Assert
            objectMother.querySms.VerifyAllExpectations();

            Assert.IsInstanceOf<SentMessageIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as SentMessageIndexOuputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(5, jsonData.TotalItems);
        }
    }
}
