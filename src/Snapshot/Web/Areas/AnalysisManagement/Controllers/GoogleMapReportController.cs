using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Areas.AnalysisManagement.Models.GoogleMapReport;
using Web.Security;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class GoogleMapReportController : Controller
    {
        public IQueryService<Outpost> QueryOutposts { get; set; }
        public IQueryService<District> QueryDistricts { get; set; }
        public IQueryService<Region> QueryRegions { get; set; }
        public IQueryService<MessageFromDrugShop> QueryMessageFromDrugShops { get; set; }
        public IQueryService<MessageFromDispensary> QueryMessageFromDispensarys { get; set; }

        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        private Client _client;
        private User _user;

        private string NOTAVAILABLE = "N";
        private string NOTREATMENT = "NP";
        private string WEEKS4 = "NS4";
        private string NOARRIVAL = "NSA";

        [HttpGet]
        [Requires(Permissions = "Report.View")]
        public ActionResult Overview()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetOutpostMarkers(Guid countryId)
        {
            LoadUserAndClient();
            
            var outpostDataQuery = QueryOutposts.Query().Where(it => it.Client == _client && it.Country.Id == countryId);
            var totalItems = outpostDataQuery.Count();

            var outpostModelListProjection = (from outpost in outpostDataQuery.ToList()
                                              select new MarkerModel
                                              {
                                                  Name = outpost.Name,
                                                  Number = GetNumberOfPatientsFor(outpost).ToString(),
                                                  Type = SetType(outpost.OutpostType.Type),
                                                  Coordonates = outpost.Latitude,
                                                  FemaleYounger = GetNumberOfPatientsFor(outpost, "F", 0, 20).ToString(),
                                                  FemaleOlder = GetNumberOfPatientsFor(outpost, "F", 1, 20).ToString(),
                                                  MaleYounger = GetNumberOfPatientsFor(outpost, "M", 0, 20).ToString(),
                                                  MaleOlder = GetNumberOfPatientsFor(outpost, "M", 1, 20).ToString(),

                                              }).ToArray();

            return Json(new MarkerIndexOutputModel
            {
                Markers = outpostModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        private int GetNumberOfPatientsFor(Outpost outpost, string gender, int younger, int age)
        {
            if (outpost.OutpostType.Type == 0)
            {
                var query = QueryMessageFromDrugShops.Query().Where(it => it.OutpostId == outpost.Id);
                if (!string.IsNullOrEmpty(gender))
                    query = query.Where(it => it.Gender.ToUpper() == gender);
                if (younger == 0)
                    query = query.Where(it => it.BirthDate >= DateTime.UtcNow.AddYears(-age));
                else
                    query = query.Where(it => it.BirthDate < DateTime.UtcNow.AddYears(-age));
                return query.Count();
            }

            if (outpost.OutpostType.Type == 1 || outpost.OutpostType.Type == 2)
            {
                var query = QueryMessageFromDispensarys.Query().Where(it => it.OutpostId == outpost.Id);
                if (!string.IsNullOrEmpty(gender))
                    query = query.Where(it => it.MessageFromDrugShop.Gender.ToUpper() == gender);
                if (younger == 0)
                    query = query.Where(it => it.MessageFromDrugShop.BirthDate >= DateTime.UtcNow.AddYears(-age));
                else
                    query = query.Where(it => it.MessageFromDrugShop.BirthDate < DateTime.UtcNow.AddYears(-age));
                return query.Count();
            }

            return 0;
        }

        private string SetType(int type)
        {
            if (type == 0)
                return "drugshop";
            if (type == 1)
                return "dispensary";

            return "healthcenter";
        }

        private int GetNumberOfPatientsFor(Outpost outpost)
        {
            if (outpost.OutpostType.Type == 0)
                return QueryMessageFromDrugShops.Query().Where(it => it.OutpostId == outpost.Id).Count();

            if (outpost.OutpostType.Type == 1 || outpost.OutpostType.Type == 2)
            {
                var queryMessage = QueryMessageFromDispensarys.Query().Where(it => it.OutpostId == outpost.Id);
                queryMessage = queryMessage.Where(it => it.Treatments.FirstOrDefault(c => c.Code == NOTAVAILABLE) == null);
                queryMessage = queryMessage.Where(it => it.Treatments.FirstOrDefault(c => c.Code == NOTREATMENT) == null);
                queryMessage = queryMessage.Where(it => it.Treatments.FirstOrDefault(c => c.Code == WEEKS4) == null);
                queryMessage = queryMessage.Where(it => it.Treatments.FirstOrDefault(c => c.Code == NOARRIVAL) == null);

                return queryMessage.Count();
            }

            return 0;
        }

        [HttpGet]
        public JsonResult GetDistrictMarkers(Guid countryId)
        {
            LoadUserAndClient();

            var districtDataQuery = QueryDistricts.Query().Where(it => it.Client == _client && it.Region.Country.Id == countryId);

            var districtModelListProjection = (from district in districtDataQuery.ToList()
                                              select new MarkerModel
                                              {
                                                  Name = district.Name,
                                                  Number = GetNumberOfPatientsFor(district, null).ToString(),
                                                  Type = "drugshop",
                                                  Coordonates = GetCenterCoordonates(district),
                                                  FemaleYounger = GetNumberOfPatientsFor(district, null, "F", 0, 20).ToString(),
                                                  FemaleOlder = GetNumberOfPatientsFor(district, null, "F", 1, 20).ToString(),
                                                  MaleYounger = GetNumberOfPatientsFor(district, null, "M", 0, 20).ToString(),
                                                  MaleOlder = GetNumberOfPatientsFor(district, null, "M", 1, 20).ToString()
                                              }).ToArray();

            return Json(new MarkerIndexOutputModel
            {
                Markers = districtModelListProjection,
                TotalItems = districtModelListProjection.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private object GetNumberOfPatientsFor(District district, Region region, string gender, int younger, int age)
        {
            int sum = 0;
            var outpostDataQuery = QueryOutposts.Query().Where(it => it.Client == _client);

            if (district != null)
                outpostDataQuery = outpostDataQuery.Where(it => it.District.Id == district.Id);
            if (region != null)
                outpostDataQuery = outpostDataQuery.Where(it => it.Region.Id == region.Id);

            foreach (var outpost in outpostDataQuery)
                sum += GetNumberOfPatientsFor(outpost, gender, younger, age);

            return sum;
        }

        private string GetCenterCoordonates(District district)
        {
            double lat = 0;
            double lng = 0;

            var outpostDataQuery = QueryOutposts.Query().Where(it => it.Client == _client);
            if (district != null)
                outpostDataQuery = outpostDataQuery.Where(it => it.District.Id == district.Id);

            foreach (var outpost in outpostDataQuery)
            {
                lat += double.Parse(outpost.Latitude.Substring(1, outpost.Latitude.IndexOf(",")-1));
                lng += double.Parse(outpost.Latitude.Substring(outpost.Latitude.IndexOf(",")+1, outpost.Latitude.Length - outpost.Latitude.IndexOf(",")-2).Trim());
            }

            return "(" + lat / outpostDataQuery.Count() + "," + lng / outpostDataQuery.Count() + ")";
        }

        private int GetNumberOfPatientsFor(District district, Region region)
        {
            int sum = 0;
            var outpostDataQuery = QueryOutposts.Query().Where(it => it.Client == _client);

            if (district != null)
                outpostDataQuery = outpostDataQuery.Where(it => it.District.Id == district.Id);
            if (region != null)
                outpostDataQuery = outpostDataQuery.Where(it => it.Region.Id == region.Id);

            foreach (var outpost in outpostDataQuery)
                sum += GetNumberOfPatientsFor(outpost);

            return sum;
        }

        [HttpGet]
        public JsonResult GetRegionMarkers(Guid countryId)
        {
            LoadUserAndClient();

            var regionDataQuery = QueryRegions.Query().Where(it => it.Client == _client && it.Country.Id == countryId);
            var totalItems = regionDataQuery.Count();

            var districtModelListProjection = (from region in regionDataQuery.ToList()
                                               select new MarkerModel
                                               {
                                                   Name = region.Name,
                                                   Number = GetNumberOfPatientsFor(null, region).ToString(),
                                                   Type = "drugshop",
                                                   Coordonates = GetCenterCoordonates(region),
                                                   FemaleYounger = GetNumberOfPatientsFor(null, region, "F", 0, 20).ToString(),
                                                   FemaleOlder = GetNumberOfPatientsFor(null, region, "F", 1, 20).ToString(),
                                                   MaleYounger = GetNumberOfPatientsFor(null, region, "M", 0, 20).ToString(),
                                                   MaleOlder = GetNumberOfPatientsFor(null, region, "M", 1, 20).ToString()
                                               }).ToArray();

            return Json(new MarkerIndexOutputModel
            {
                Markers = districtModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetCenterCoordonates(Region region)
        {
            double lat = 0;
            double lng = 0;

            var districtDataQuery = QueryDistricts.Query().Where(it => it.Region.Id == region.Id);
            
            foreach (var district in districtDataQuery)
            {
                var coordonates = GetCenterCoordonates(district);
                lat += double.Parse(coordonates.Substring(1, coordonates.IndexOf(",") - 1));
                lng += double.Parse(coordonates.Substring(coordonates.IndexOf(",") + 1, coordonates.Length - coordonates.IndexOf(",") - 2).Trim());
            }

            return "(" + lat / districtDataQuery.Count() + "," + lng / districtDataQuery.Count() + ")";
        }


        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            this._user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null)
                throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = QueryClients.Load(clientId);
        }

    }
}
