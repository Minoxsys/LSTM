using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.ConditionManagement
{
    public class ConditionManagementAreaRegistration : AreaRegistration
    {
        public const string DEFAULT_ROUTE = "ConditionManagement_default";
        public override string AreaName
        {
            get
            {
                return "ConditionManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                DEFAULT_ROUTE,
                "ConditionManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}