using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Domain;
using Core.Persistence;
using Domain;
using Web.Security;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class PatientDemographicsController : Controller
    {
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        private Client _client;
        private User _user;


        [HttpGet]
        [Requires(Permissions = "Report.View")]
        public ActionResult Overview()
        {
            return View();
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