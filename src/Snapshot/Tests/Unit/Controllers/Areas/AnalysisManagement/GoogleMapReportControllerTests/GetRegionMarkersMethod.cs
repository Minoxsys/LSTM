using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using Rhino.Mocks;
using Web.Areas.AnalysisManagement.Models.GoogleMapReport;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.GoogleMapReportControllerTests
{
    [TestFixture]
    public class GetRegionMarkersMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_WithAListOf_Markers()
        {
            //Arrange
            objectMother.queryRegion.Expect(call => call.Query()).Return(new Region[] { objectMother.region }.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(new Outpost[] { objectMother.drugshop }.AsQueryable());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.ListOfMessageFromDrugShop());

            //Act
            var jsonResult = objectMother.controller.GetRegionMarkers(objectMother.countryId);

            //Assert
            objectMother.queryRegion.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
            Assert.AreEqual("10", jsonData.Markers[0].Number);
            Assert.AreEqual(objectMother.drugshop.Latitude, jsonData.Markers[0].Coordonates);
        }
    }
}
