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
        public void Returns_JSON_WithAListOf_Markers()
        {
            //Arrange
            objectMother.queryDistrict.Expect(call => call.Query()).Return(new District[] { objectMother.district }.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(new Outpost[] { objectMother.drugshop, objectMother.dispensary }.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.ListOfMessageFromDispensary());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.ListOfMessageFromDrugShop());

            //Act
            var jsonResult = objectMother.controller.GetDistrictMarkers(objectMother.countryId);

            //Assert
            objectMother.queryDistrict.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<MarkerIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as MarkerIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
            Assert.AreEqual("20", jsonData.Markers[0].Number);
            Assert.AreEqual("(23.1787678875,16.1110010565)", jsonData.Markers[0].Coordonates);
        }
    }
}
