using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.MessagesManagement.Controllers;
using Web.Areas.MessagesManagement.Models.Messages;
using Web.Services;

namespace Tests.Unit.Controllers.Areas.MessagesManagement.DispensaryControllerTests
{
    [TestFixture]
    public class GetMessagesFromDispensaryMethod
    {
        private readonly ObjectMother _objectMother = new ObjectMother();
        private DispensaryController _ctrl;

        [SetUp]
        public void BeforeAll()
        {
            _objectMother.Init();
            _ctrl = new DispensaryController {MessagesService = new MessagesService(_objectMother.queryRawSms, _objectMother.queryOutposts)};
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
            var pageOfData = _objectMother.PageOfData(indexModel);
            _objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = _ctrl.GetMessagesFromDispensary(indexModel);

            //Assert
            _objectMother.queryRawSms.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MessageIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_OnlyMessagesFromDispensary()
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
            var pageOfData = _objectMother.PageOfDispensaryData(indexModel);
            _objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = _ctrl.GetMessagesFromDispensary(indexModel);

            //Assert
            _objectMother.queryRawSms.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MessageIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count()/2, jsonData.TotalItems);
        }

        [Test]
        public void Returns_OnlyMessagesFromDispensary_OrderDESC_byContent()
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
            var pageOfData = _objectMother.PageOfDispensaryData(indexModel);
            _objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = _ctrl.GetMessagesFromDispensary(indexModel);

            //Assert
            _objectMother.queryRawSms.VerifyAllExpectations();

            var jsonData = jsonResult.Data as MessageIndexOuputModel;
            Assert.NotNull(jsonData);
            Assert.That(jsonData.Messages[0].Content, Is.EqualTo("123456 Tr1 Tr2-9"));
        }

        [Test]
        public void Returns_OnlyMessagesFromDispensaries_WhereContentContains_SearchValue()
        {
            //Arrange
            var indexModel = new MessagesIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Sender",
                searchValue = "-9"
            };

            var pageOfData = _objectMother.PageOfDispensaryData(indexModel);
            _objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = _ctrl.GetMessagesFromDispensary(indexModel);

            //Assert
            _objectMother.queryRawSms.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MessageIndexOuputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(1, jsonData.TotalItems);
        }
    }
}
