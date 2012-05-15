using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.SmsRequest;
using Core.Persistence;
using Domain;
using Web.Services;
using Core.Domain;

namespace Web.Controllers
{
    public class SmsRequestController : Controller
    {
        public IQueryService<User> QueryUsers { get; set; }
        public IQueryService<Client> QueryClients { get; set; }

        public IQueryService<Outpost> QueryOutpost { get; set; }

        public ISmsRequestService SmsRequestService { get; set; }
        public ISmsGatewayService SmsGatewayService { get; set; }

        public ISaveOrUpdateCommand<RawSmsReceived> SaveCommandRawSmsReceived { get; set; }

        private Client _client;
        private User _user;

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            this._user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null) throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = QueryClients.Load(clientId);
        }

        public ActionResult Create()
        {
            LoadUserAndClient();
            SmsRequestCreateModel model = new SmsRequestCreateModel();

            return View(model);
        }

        public ActionResult Overview()
        {
            var model = new SmsRequestCreateModel();
            return View(model);
        }

   

        public ActionResult ReceiveSms(string sender, string content, DateTime date, string inNumber, string email, string credits)
        {
            RawSmsReceived rawSmsReceived = new RawSmsReceived();
            rawSmsReceived.Sender = sender;
            rawSmsReceived.Content = content;
            rawSmsReceived.Credits = credits;
            rawSmsReceived.Date = date;
            rawSmsReceived = SmsGatewayService.AssignOutpostToRawSmsReceivedBySenderNumber(rawSmsReceived);

            //SaveCommandRawSmsReceived.Execute(rawSmsReceived);

            RawSmsReceivedParseResult parseResult = SmsGatewayService.ParseRawSmsReceived(rawSmsReceived);
            rawSmsReceived.ParseSucceeded = parseResult.ParseSucceeded;

            SaveCommandRawSmsReceived.Execute(rawSmsReceived);

            if (parseResult.ParseSucceeded)
                SmsRequestService.UpdateOutpostStockLevelsWithValuesReceivedBySms(parseResult.SmsReceived);

            return null;
        }

    }
}
