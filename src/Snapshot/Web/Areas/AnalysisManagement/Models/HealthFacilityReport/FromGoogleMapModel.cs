using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.HealthFacilityReport
{
    public class FromGoogleMapModel
    {
        public Guid CountryId { get; set; }
        public Guid RegionId { get; set; }
        public Guid DistrictId { get; set; }
        public int TypeId { get; set; }
    }
}