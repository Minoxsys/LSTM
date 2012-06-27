using System.Web.Mvc;

namespace Web.Areas.LocationManagement
{
    public class LocationManagementAreaRegistration : AreaRegistration
    {
        public const string DEFAULT_ROUTE = "LocationManagement_default";
        public override string AreaName
        {
            get
            {
                return "LocationManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                DEFAULT_ROUTE,
                "LocationManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
