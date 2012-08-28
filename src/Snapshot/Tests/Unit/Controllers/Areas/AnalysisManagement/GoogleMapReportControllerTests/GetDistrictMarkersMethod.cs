using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.AnalysisManagement.Models.GoogleMapReport;
using Domain;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.GoogleMapReportControllerTests
{
    [TestFixture]
    public class GetDistrictMarkersMethod
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
            objectMother.queryDistrict.Expect(call => call.Query()).Return(new District[] { objectMother.district1 }.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(new Outpost[] { objectMother.drugshop, objectMother.dispensary }.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.ListOfMessageFromDispensary());
            FilterModel model = new FilterModel { countryId = objectMother.countryId };

            //Act
            var jsonResult = objectMother.controller.GetDistrictMarkers(model);

            //Assert
            objectMother.queryDistrict.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
            Assert.AreEqual("9", jsonData.Markers[0].Number);
            Assert.AreEqual(objectMother.dispensary.Latitude, jsonData.Markers[0].Coordonates);
        }

        [Test]
        public void Returns_JSON_WithAListOf_Markers_FilterdBy_Region()
        {
            //Arrange
            objectMother.queryDistrict.Expect(call => call.Query()).Return(new District[] { objectMother.district2 }.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(new Outpost[] { objectMother.drugshop }.AsQueryable());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.ListOfMessageFromDrugShop());
            FilterModel model = new FilterModel { countryId = objectMother.countryId, regionId = objectMother.regionId };

            //Act
            var jsonResult = objectMother.controller.GetDistrictMarkers(model);

            //Assert
            objectMother.queryDistrict.VerifyAllExpectations();
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

        [Test]
        public void Returns_JSON_WithAListOf_Markers_FilterdBy_District()
        {
            //Arrange
            objectMother.queryDistrict.Expect(call => call.Query()).Return(new District[] { objectMother.district1 }.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(new Outpost[] { objectMother.drugshop, objectMother.dispensary }.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.ListOfMessageFromDispensary());
            FilterModel model = new FilterModel { countryId = objectMother.countryId, regionId = objectMother.regionId, districtId = objectMother.district1Id };

            //Act
            var jsonResult = objectMother.controller.GetDistrictMarkers(model);

            //Assert
            objectMother.queryDistrict.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
            Assert.AreEqual("9", jsonData.Markers[0].Number);
            Assert.AreEqual(objectMother.dispensary.Latitude, jsonData.Markers[0].Coordonates);
        }
    }
}
