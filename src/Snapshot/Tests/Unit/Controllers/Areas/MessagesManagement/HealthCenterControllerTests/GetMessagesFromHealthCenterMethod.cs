using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.MessagesManagement.Controllers;
using Web.Areas.MessagesManagement.Models.Messages;
using Web.Services;

namespace Tests.Unit.Controllers.Areas.MessagesManagement.HealthCenterControllerTests
{
    [TestFixture]
    public class GetMessagesFromHealthCenterMethod
    {
        private readonly ObjectMother _objectMother = new ObjectMother();
        private HealthCenterController _ctrl;

        [SetUp]
        public void BeforeEach()
        {
            _objectMother.Init();
            _ctrl = new HealthCenterController {MessagesService = new MessagesService(_objectMother.queryRawSms, _objectMother.queryOutposts)};
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
            var jsonResult = _ctrl.GetMessagesFromHealthCenter(indexModel);

            //Assert
            _objectMother.queryRawSms.VerifyAllExpectations();

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
            var pageOfData = _objectMother.PageOfDrugShopData(indexModel);
            _objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = _ctrl.GetMessagesFromHealthCenter(indexModel);

            //Assert
            _objectMother.queryRawSms.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MessageIndexOuputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count()/3, jsonData.TotalItems);
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
            var pageOfData = _objectMother.PageOfDrugShopData(indexModel);
            _objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = _ctrl.GetMessagesFromHealthCenter(indexModel);

            //Assert
            _objectMother.queryRawSms.VerifyAllExpectations();

            var jsonData = jsonResult.Data as MessageIndexOuputModel;
            Assert.NotNull(jsonData);
            Assert.That(jsonData.Messages[0].Content, Is.EqualTo(_objectMother.rawSms.Content + "-8"));
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

            var pageOfData = _objectMother.PageOfDrugShopData(indexModel);
            _objectMother.queryRawSms.Expect(call => call.Query()).Return(pageOfData);

            //Act
            var jsonResult = _ctrl.GetMessagesFromHealthCenter(indexModel);

            //Assert
            _objectMother.queryRawSms.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageIndexOuputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MessageIndexOuputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(1, jsonData.TotalItems);
        }
    }
}
