using System.Web.Mvc;

namespace Web.Areas.MessagesManagement
{
    public class MessagesManagementAreaRegistration : AreaRegistration
    {
        public const string DEFAULT_ROUTE = "MessagesManagement_default";
        public override string AreaName
        {
            get
            {
                return "MessagesManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                DEFAULT_ROUTE,
                "MessagesManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
