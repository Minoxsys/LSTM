using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Security;

namespace Web.Areas.ServicesManagement.Controllers
{
    public class DiagnosisController : Controller
    {
        [HttpGet]
        //[Requires(Permissions = "Diagnosis.View")]
        public ActionResult Overview()
        {
            //ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(REGION_ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            //ViewBag.HasNoRightsToDelete = (PermissionService.HasPermissionAssigned(REGION_DELETE_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();

            return View("Overview");
        }

    }
}
