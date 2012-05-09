using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.MessagesManagement.Controllers
{
    public class DrugstoreController : Controller
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
