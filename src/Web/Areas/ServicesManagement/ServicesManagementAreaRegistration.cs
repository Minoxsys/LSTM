using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ServicesManagement
{
    public class ServicesManagementAreaRegistration : AreaRegistration
    {
        public const string DEFAULT_ROUTE = "ServicesManagement_default";
        public override string AreaName
        {
            get
            {
                return "ServicesManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                DEFAULT_ROUTE,
                "ServicesManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}