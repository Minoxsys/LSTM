using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.HealthFacilityReportControllerTests
{
    [TestFixture]
    public class FromGoogleMapMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Redirects_To_Overview()
        {
            //Arrange
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.outpostId)).Return(objectMother.outpost);

            // Act
            var redirectResult = (RedirectToRouteResult)objectMother.controller.FromGoogleMap(objectMother.outpostId, "outpost", 0);

            // Assert
            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
        }
    }
}
