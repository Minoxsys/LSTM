using System;
using System.Linq;
using NUnit.Framework;
using System.Web.Mvc;
using Web.Areas.LocationManagement.Models.Outpost;


namespace Tests.Unit.Controllers.Areas.LocationManagement.OutpostControllerTests
{

    [TestFixture]
    public class OverviewMethod
    {
        public readonly ObjectMother _ = new ObjectMother();
        [SetUp]
        public void BeforeEach()
        {
            _.Init();
        }
        [Test]
        public void Returns_The_DefaultView()
        {
            //Arrange

            // Act
            var viewResult = (ViewResult)_.controller.Overview();

            // Assert
            Assert.AreEqual("Overview", viewResult.ViewName);
            Assert.IsInstanceOf<OutpostOverviewModel>(viewResult.Model);
        }
    }
}
