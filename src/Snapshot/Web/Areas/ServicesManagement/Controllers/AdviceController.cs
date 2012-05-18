using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Areas.ServicesManagement.Models.Advice;
using Web.Models.Shared;
using AutoMapper;

namespace Web.Areas.ServicesManagement.Controllers
{
    public class AdviceController : Controller
    {
        public IQueryService<Advice> QueryAdvice { get; set; }

        public ISaveOrUpdateCommand<Advice> SaveOrUpdateCommand { get; set; }
        public IDeleteCommand<Advice> DeleteCommand { get; set; }

        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        private Client _client;
        private User _user;


        [HttpGet]
        //[Requires(Permissions = "Advice.View")]
        public ActionResult Overview()
        {
            //ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(REGION_ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            //ViewBag.HasNoRightsToDelete = (PermissionService.HasPermissionAssigned(REGION_DELETE_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();

            return View("Overview");
        }

        [HttpPost]
        public JsonResult Create(AdviceModel adviceInputModel)
        {
            LoadUserAndClient();

            if (string.IsNullOrEmpty(adviceInputModel.Code))
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "The advice has not been saved!"
                   });
            }

            var queryAdviceValidation = QueryAdvice.Query().Where(p => p.Client == _client);
            if (queryAdviceValidation.Where(it => it.Code == adviceInputModel.Code && it.Keyword == adviceInputModel.Keyword).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a advice with code {0} and service {1}", adviceInputModel.Code, adviceInputModel.Keyword)
                    });

            }

            var advice = new Advice();
            CreateMappings();
            Mapper.Map(adviceInputModel, advice);
            advice.Client = this._client;
            advice.ByUser = this._user;

            SaveOrUpdateCommand.Execute(advice);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Advice {0} has been saved.", advice.Code)
               });
        }

        [HttpPost]
        public JsonResult Edit(AdviceModel adviceInputModel)
        {
            LoadUserAndClient();

            if (adviceInputModel.Id == Guid.Empty)
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "You must supply a adviceId in order to edit the advice."
                   });
            }

            if (string.IsNullOrEmpty(adviceInputModel.Code))
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "The advice has not been saved!"
                   });
            }

            var queryAdviceValidation = QueryAdvice.Query().Where(p => p.Client == _client);

            if (queryAdviceValidation.Where(it =>
                                                    it.Code == adviceInputModel.Code &&
                                                    it.Keyword == adviceInputModel.Keyword &&
                                                    it.Id != adviceInputModel.Id).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a advice with code {0} and service {1}", adviceInputModel.Code, adviceInputModel.Keyword)
                    });

            }

            var advice = new Advice();
            CreateMappings();
            Mapper.Map(adviceInputModel, advice);
            advice.Client = this._client;
            advice.ByUser = this._user;

            SaveOrUpdateCommand.Execute(advice);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Advice {0} has been saved.", advice.Code)
               });
        }

        [HttpPost]
        public JsonResult Delete(Guid? id)
        {
            LoadUserAndClient();

            if (id.HasValue == false)
            {
                return Json(new JsonActionResponse
                {
                    Status = "Error",
                    Message = "You must supply a adviceId in order to remove the advice."
                });
            }

            var advice = QueryAdvice.Load(id.Value);
            if (advice != null)
            {
                DeleteCommand.Execute(advice);
            }

            return Json(
                new JsonActionResponse
                {
                    Status = "Success",
                    Message = String.Format("Advice with code {0} was removed.", advice.Code)
                });
        }

        [HttpGet]
        public JsonResult GetAdvices(AdviceIndexModel indexModel)
        {
            LoadUserAndClient();

            var pageSize = indexModel.limit.Value;
            var adviceDataQuery = this.QueryAdvice.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Advice>>>()
            {
                { "Code-ASC", () => adviceDataQuery.OrderBy(c => c.Code) },
                { "Code-DESC", () => adviceDataQuery.OrderByDescending(c => c.Code) },
                { "Keyword-ASC", () => adviceDataQuery.OrderBy(c => c.Keyword) },
                { "Keyword-DESC", () => adviceDataQuery.OrderByDescending(c => c.Keyword) },
                { "Description-ASC", () => adviceDataQuery.OrderBy(c => c.Description) },
                { "Description-DESC", () => adviceDataQuery.OrderByDescending(c => c.Description) }
            };

            adviceDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();
            adviceDataQuery = adviceDataQuery.Where(it => it.Client.Id == this._client.Id);

            if (!string.IsNullOrEmpty(indexModel.searchValue))
                adviceDataQuery = adviceDataQuery.Where(it => it.Code.Contains(indexModel.searchValue));

            var totalItems = adviceDataQuery.Count();

            adviceDataQuery = adviceDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var adviceModelListProjection = (from advice in adviceDataQuery.ToList()
                                                select new AdviceModel
                                                {
                                                    Id = advice.Id,
                                                    Keyword = advice.Keyword,
                                                    Code = advice.Code,
                                                    Description = advice.Description
                                                }).ToArray();


            return Json(new AdviceIndexOuputModel
            {
                Advices = adviceModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
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

        private void CreateMappings()
        {
            Mapper.CreateMap<AdviceModel, Advice>();
            Mapper.CreateMap<Advice, AdviceModel>();
        }

    }
}
