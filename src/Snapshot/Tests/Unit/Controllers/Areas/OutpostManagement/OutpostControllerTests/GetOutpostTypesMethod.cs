using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Outpost;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.OutpostControllerTests
{
    [TestFixture]
    public class GetOutpostTypesMethod
    {
        public readonly ObjectMother _ = new ObjectMother();
        [SetUp]
        public void BeforeEach()
        {
            _.Init();
            _.StubUserAndItsClient();
        }

        [TearDown]
        public void AfterEach()
        {
            _.VerifyUserAndClientExpectations();
        }

        [Test]
        public void RequiresALoggedInUserAndAClient()
        {
            var viewResult = _.controller.GetOutpostTypes() as JsonResult;

            Assert.IsNotNull(viewResult);
        }

        [Test]
        public void NeverReturnsNull()
        {
            var viewResult = _.controller.GetOutpostTypes() as JsonResult;

            Assert.IsNotNull(viewResult.Data);
            Assert.IsInstanceOf<OutpostTypesIndexOutputModel>(viewResult.Data);
        }

        [Test]
        public void ReturnsOutpostTypesArray()
        {
            var returnedOutpostTypes = _.ExpectOutpostTypesToBeQueried();

            var viewResult = _.controller.GetOutpostTypes() as JsonResult;
            var model = viewResult.Data as OutpostTypesIndexOutputModel;

            _.VerifyThatOutpostTypesHaveBeenQueried();
            Assert.AreEqual(returnedOutpostTypes.Count(), model.OutpostTypes.Count());
            Assert.AreEqual(returnedOutpostTypes[0].Id, model.OutpostTypes[0].Id);
            Assert.AreEqual(returnedOutpostTypes[0].Name, model.OutpostTypes[0].Name);
        }

        
    }
}
