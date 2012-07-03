using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Areas.ConditionManagement.Models.Condition;
using AutoMapper;
using Web.Models.Shared;
using Core.Security;
using Web.Security;

namespace Web.Areas.ConditionManagement.Controllers
{
    public class ConditionController : Controller
    {
        public IQueryService<Condition> QueryCondition { get; set; }
        public ISaveOrUpdateCommand<Condition> SaveOrUpdateCommand { get; set; }
        public IDeleteCommand<Condition> DeleteCommand { get; set; }
       
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        
        private Client _client;
        private User _user;

        public IPermissionsService PermissionService { get; set; }
        private string ADD_PERMISSION = "Condition.Edit";
        private string DELETE_PERMISSION = "Condition.Delete";

        [HttpGet]
        [Requires(Permissions = "Condition.View")]
        public ActionResult Overview()
        {
            ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            ViewBag.HasNoRightsToDelete = (PermissionService.HasPermissionAssigned(DELETE_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();

            return View("Overview");
        }

        [HttpPost]
        public JsonResult Create(ConditionModel conditionInputModel)
        {
            LoadUserAndClient();

            if (string.IsNullOrEmpty(conditionInputModel.Code))
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "The symptom has not been saved!"
                   });
            }

            var queryConditionValidation = QueryCondition.Query().Where(p => p.Client == _client);
            if (queryConditionValidation.Where(it => it.Code == conditionInputModel.Code && it.Keyword == conditionInputModel.Keyword).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a symptom with code {0} and keyword {1}", conditionInputModel.Code, conditionInputModel.Keyword)
                    });

            }

            var condition = new Condition();
            CreateMappings();
            Mapper.Map(conditionInputModel, condition);
            condition.Client = this._client;
            condition.ByUser = this._user;

            SaveOrUpdateCommand.Execute(condition);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Symptom {0} has been saved.", condition.Code)
               });
        }

        [HttpPost]
        public JsonResult Edit(ConditionModel conditionInputModel)
        {
            LoadUserAndClient();

            if (conditionInputModel.Id == Guid.Empty)
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "You must supply a symptomId in order to edit the symptom."
                   });
            }

            if (string.IsNullOrEmpty(conditionInputModel.Code))
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "The symptom has not been saved!"
                   });
            }

            var queryConditionValidation = QueryCondition.Query().Where(p => p.Client == _client);

            if (queryConditionValidation.Where(it =>
                                                    it.Code == conditionInputModel.Code &&
                                                    it.Keyword == conditionInputModel.Keyword &&
                                                    it.Id != conditionInputModel.Id).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a symptom with code {0} and keyword {1}", conditionInputModel.Code, conditionInputModel.Keyword)
                    });

            }

            var condition = new Condition();
            CreateMappings();
            Mapper.Map(conditionInputModel, condition);
            condition.Client = this._client;
            condition.ByUser = this._user;

            SaveOrUpdateCommand.Execute(condition);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Symptom {0} has been saved.", condition.Code)
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
                    Message = "You must supply a symptomId in order to remove the condition."
                });
            }

            var condition = QueryCondition.Load(id.Value);
            if (condition != null)
            {
                DeleteCommand.Execute(condition);
            }

            return Json(
                new JsonActionResponse
                {
                    Status = "Success",
                    Message = String.Format("Symptom with code {0} was removed.", condition.Code)
                });
        }

        [HttpGet]
        public JsonResult GetCondition(ConditionIndexModel indexModel)
        {
            LoadUserAndClient();

            var pageSize = indexModel.limit.Value;
            var conditionDataQuery = this.QueryCondition.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Condition>>>()
            {
                { "Code-ASC", () => conditionDataQuery.OrderBy(c => c.Code) },
                { "Code-DESC", () => conditionDataQuery.OrderByDescending(c => c.Code) },
                { "Keyword-ASC", () => conditionDataQuery.OrderBy(c => c.Keyword) },
                { "Keyword-DESC", () => conditionDataQuery.OrderByDescending(c => c.Keyword) },
                { "Description-ASC", () => conditionDataQuery.OrderBy(c => c.Description) },
                { "Description-DESC", () => conditionDataQuery.OrderByDescending(c => c.Description) }
            };

            conditionDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();
            conditionDataQuery = conditionDataQuery.Where(it => it.Client.Id == this._client.Id);

            if (!string.IsNullOrEmpty(indexModel.searchValue))
                conditionDataQuery = conditionDataQuery.Where(it => it.Code.Contains(indexModel.searchValue));

            var totalItems = conditionDataQuery.Count();

            conditionDataQuery = conditionDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var conditionModelListProjection = (from condition in conditionDataQuery.ToList()
                                                select new ConditionModel
                                                {
                                                    Id = condition.Id,
                                                    Keyword = condition.Keyword,
                                                    Code = condition.Code,
                                                    Description = condition.Description
                                                }).ToArray();


            return Json(new ConditionIndexOuputModel
            {
                Condition = conditionModelListProjection,
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
            Mapper.CreateMap<ConditionModel, Condition>();
            Mapper.CreateMap<Condition, ConditionModel>();
        }

    }
}
