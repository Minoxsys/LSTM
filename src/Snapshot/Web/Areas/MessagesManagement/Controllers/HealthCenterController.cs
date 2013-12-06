using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Web.Areas.MessagesManagement.Models.Messages;
using Web.Security;
using Web.Services;

namespace Web.Areas.MessagesManagement.Controllers
{
    public class HealthCenterController : Controller
    {
        public IQueryService<RawSmsReceived> QueryRawSms { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IMessagesService MessagesService { get; set; }

        [HttpGet]
        [Requires(Permissions = "Messages.View")]
        public ActionResult Overview()
        {
            return View("Overview");
        }

        [HttpGet]
        public JsonResult GetMessagesFromHealthCenter(MessagesIndexModel indexModel)
        {
            return Json(MessagesService.GetMessagesFromOutpost(indexModel, 2), JsonRequestBehavior.AllowGet);
        }
    }
}
