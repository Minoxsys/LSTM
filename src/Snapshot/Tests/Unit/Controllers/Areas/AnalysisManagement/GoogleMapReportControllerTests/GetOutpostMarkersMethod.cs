using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Areas.AnalysisManagement.Models.GoogleMapReport;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.GoogleMapReportControllerTests
{
    [TestFixture]
    public class GetOutpostMarkersMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_WithAListOf_Markers_FilterdBy_Country()
        {
            //Arrange
            objectMother.queryOutpost.Expect(call => call.Query()).Return(new Outpost[] { objectMother.drugshop, objectMother.dispensary }.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.ListOfMessageFromDispensary());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.ListOfMessageFromDrugShop());
            FilterModel model = new FilterModel { countryId = objectMother.countryId };


            //Act
            var jsonResult = objectMother.controller.GetOutpostMarkers(model);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(2, jsonData.TotalItems);
            Assert.AreEqual("10", jsonData.Markers[0].Number);
            Assert.AreEqual("9", jsonData.Markers[1].Number);
        }

        [Test]
        public void Returns_JSON_WithAListOf_Markers_FilterdBy_Region()
        {
            //Arrange
            objectMother.queryOutpost.Expect(call => call.Query()).Return(new Outpost[] { objectMother.outpost22 }.AsQueryable());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.ListOfMessageFromDrugShop());
            FilterModel model = new FilterModel { countryId = objectMother.countryId, regionId = objectMother.regionId2 };


            //Act
            var jsonResult = objectMother.controller.GetOutpostMarkers(model);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
            Assert.AreEqual("1", jsonData.Markers[0].Number);
        }

        [Test]
        public void Returns_JSON_WithAListOf_Markers_FilterdBy_District()
        {
            //Arrange
            objectMother.queryOutpost.Expect(call => call.Query()).Return(new Outpost[] { objectMother.outpost22 }.AsQueryable());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.ListOfMessageFromDrugShop());
            FilterModel model = new FilterModel { countryId = objectMother.countryId, regionId = objectMother.regionId2, districtId = objectMother.districtId22 };


            //Act
            var jsonResult = objectMother.controller.GetOutpostMarkers(model);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
            Assert.AreEqual("1", jsonData.Markers[0].Number);
        }
    }
}
