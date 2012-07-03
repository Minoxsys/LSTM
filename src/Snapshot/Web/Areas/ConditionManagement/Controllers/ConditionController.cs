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
        public IQueryService<ServiceNeeded> QueryServiceNeeded { get; set; }
        public ISaveOrUpdateCommand<ServiceNeeded> SaveOrUpdateCommand { get; set; }
        public IDeleteCommand<ServiceNeeded> DeleteCommand { get; set; }
       
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        
        private Client _client;
        private User _user;

        public IPermissionsService PermissionService { get; set; }
        private string ADD_PERMISSION = "ServiceNeeded.Edit";
        private string DELETE_PERMISSION = "ServiceNeeded.Delete";

        [HttpGet]
        [Requires(Permissions = "ServiceNeeded.View")]
        public ActionResult Overview()
        {
            ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            ViewBag.HasNoRightsToDelete = (PermissionService.HasPermissionAssigned(DELETE_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();

            return View("Overview");
        }

        [HttpPost]
        public JsonResult Create(ServiceNeededModel serviceNeededInputModel)
        {
            LoadUserAndClient();

            if (string.IsNullOrEmpty(serviceNeededInputModel.Code))
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "The symptom has not been saved!"
                   });
            }

            var queryServiceNeededValidation = QueryServiceNeeded.Query().Where(p => p.Client == _client);
            if (queryServiceNeededValidation.Where(it => it.Code == serviceNeededInputModel.Code && it.Keyword == serviceNeededInputModel.Keyword).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a symptom with code {0} and keyword {1}", serviceNeededInputModel.Code, serviceNeededInputModel.Keyword)
                    });

            }

            var serviceNeeded = new ServiceNeeded();
            CreateMappings();
            Mapper.Map(serviceNeededInputModel, serviceNeeded);
            serviceNeeded.Client = this._client;
            serviceNeeded.ByUser = this._user;

            SaveOrUpdateCommand.Execute(serviceNeeded);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Symptom {0} has been saved.", serviceNeeded.Code)
               });
        }

        [HttpPost]
        public JsonResult Edit(ServiceNeededModel serviceNeededInputModel)
        {
            LoadUserAndClient();

            if (serviceNeededInputModel.Id == Guid.Empty)
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "You must supply a symptomId in order to edit the condition."
                   });
            }

            if (string.IsNullOrEmpty(serviceNeededInputModel.Code))
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "The symptom has not been saved!"
                   });
            }

            var queryServiceNeededValidation = QueryServiceNeeded.Query().Where(p => p.Client == _client);

            if (queryServiceNeededValidation.Where(it =>
                                                    it.Code == serviceNeededInputModel.Code &&
                                                    it.Keyword == serviceNeededInputModel.Keyword &&
                                                    it.Id != serviceNeededInputModel.Id).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a symptom with code {0} and keyword {1}", serviceNeededInputModel.Code, serviceNeededInputModel.Keyword)
                    });

            }

            var serviceNeeded = new ServiceNeeded();
            CreateMappings();
            Mapper.Map(serviceNeededInputModel, serviceNeeded);
            serviceNeeded.Client = this._client;
            serviceNeeded.ByUser = this._user;

            SaveOrUpdateCommand.Execute(serviceNeeded);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Symptom {0} has been saved.", serviceNeeded.Code)
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

            var serviceNeeded = QueryServiceNeeded.Load(id.Value);
            if (serviceNeeded != null)
            {
                DeleteCommand.Execute(serviceNeeded);
            }

            return Json(
                new JsonActionResponse
                {
                    Status = "Success",
                    Message = String.Format("Symptom with code {0} was removed.", serviceNeeded.Code)
                });
        }

        [HttpGet]
        public JsonResult GetServiceNeeded(ServiceNeededIndexModel indexModel)
        {
            LoadUserAndClient();

            var pageSize = indexModel.limit.Value;
            var serviceNeededDataQuery = this.QueryServiceNeeded.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<ServiceNeeded>>>()
            {
                { "Code-ASC", () => serviceNeededDataQuery.OrderBy(c => c.Code) },
                { "Code-DESC", () => serviceNeededDataQuery.OrderByDescending(c => c.Code) },
                { "Keyword-ASC", () => serviceNeededDataQuery.OrderBy(c => c.Keyword) },
                { "Keyword-DESC", () => serviceNeededDataQuery.OrderByDescending(c => c.Keyword) },
                { "Description-ASC", () => serviceNeededDataQuery.OrderBy(c => c.Description) },
                { "Description-DESC", () => serviceNeededDataQuery.OrderByDescending(c => c.Description) }
            };

            serviceNeededDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();
            serviceNeededDataQuery = serviceNeededDataQuery.Where(it => it.Client.Id == this._client.Id);

            if (!string.IsNullOrEmpty(indexModel.searchValue))
                serviceNeededDataQuery = serviceNeededDataQuery.Where(it => it.Code.Contains(indexModel.searchValue));

            var totalItems = serviceNeededDataQuery.Count();

            serviceNeededDataQuery = serviceNeededDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var serviceNeededModelListProjection = (from serviceNeeded in serviceNeededDataQuery.ToList()
                                                select new ServiceNeededModel
                                                {
                                                    Id = serviceNeeded.Id,
                                                    Keyword = serviceNeeded.Keyword,
                                                    Code = serviceNeeded.Code,
                                                    Description = serviceNeeded.Description
                                                }).ToArray();


            return Json(new ServiceNeededIndexOuputModel
            {
                ServiceNeeded = serviceNeededModelListProjection,
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
            Mapper.CreateMap<ServiceNeededModel, ServiceNeeded>();
            Mapper.CreateMap<ServiceNeeded, ServiceNeededModel>();
        }

    }
}
