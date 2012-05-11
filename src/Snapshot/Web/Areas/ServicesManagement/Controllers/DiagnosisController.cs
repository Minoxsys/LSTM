using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Security;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Models.Shared;
using AutoMapper;
using Web.Areas.ServicesManagement.Models.Diagnosis;

namespace Web.Areas.ServicesManagement.Controllers
{
    public class DiagnosisController : Controller
    {
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        public IQueryService<Diagnosis> QueryDiagnosis { get; set; }

        public ISaveOrUpdateCommand<Diagnosis> SaveOrUpdateCommand { get; set; }
        public IDeleteCommand<Diagnosis> DeleteCommand { get; set; }

        private Client _client;
        private User _user;


        [HttpGet]
        //[Requires(Permissions = "Diagnosis.View")]
        public ActionResult Overview()
        {
            //ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(REGION_ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            //ViewBag.HasNoRightsToDelete = (PermissionService.HasPermissionAssigned(REGION_DELETE_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();

            return View("Overview");
        }

        [HttpPost]
        public JsonResult Create(DiagnosisInputModel diagnosisInputModel)
        {
            LoadUserAndClient();

            if (string.IsNullOrEmpty(diagnosisInputModel.Code))
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "The diagnosis has not been saved!"
                   });
            }

            var queryDiagnosisValidation = QueryDiagnosis.Query().Where(p => p.Client == _client);
            if (queryDiagnosisValidation.Where(it => it.Code == diagnosisInputModel.Code && it.ServiceNeeded == diagnosisInputModel.ServiceNeeded).Count() > 0 )
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a diagnosis with code {0} and service {1}", diagnosisInputModel.Code, diagnosisInputModel.ServiceNeeded)
                    });

            }

            var diagnosis = new Diagnosis();
            CreateMappings();
            Mapper.Map(diagnosisInputModel, diagnosis);
            diagnosis.Client = this._client;
            diagnosis.ByUser = this._user;

            SaveOrUpdateCommand.Execute(diagnosis);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Diagnosis {0} has been saved.", diagnosis.Code)
               });
        }

        [HttpPost]
        public JsonResult Edit(DiagnosisInputModel diagnosisInputModel)
        {
            LoadUserAndClient();

            if (diagnosisInputModel.Id == Guid.Empty)
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "You must supply a diagnosisId in order to edit the diagnosis."
                   });
            }

            if (string.IsNullOrEmpty(diagnosisInputModel.Code))
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "The diagnosis has not been saved!"
                   });
            }

            var queryDiagnosisValidation = QueryDiagnosis.Query().Where(p => p.Client == _client);

            if (queryDiagnosisValidation.Where(it => 
                                                    it.Code == diagnosisInputModel.Code && 
                                                    it.ServiceNeeded == diagnosisInputModel.ServiceNeeded &&
                                                    it.Id != diagnosisInputModel.Id).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a diagnosis with code {0} and service {1}", diagnosisInputModel.Code, diagnosisInputModel.ServiceNeeded)
                    });

            }

            var diagnosis = new Diagnosis();
            CreateMappings();
            Mapper.Map(diagnosisInputModel, diagnosis);
            diagnosis.Client = this._client;
            diagnosis.ByUser = this._user;

            SaveOrUpdateCommand.Execute(diagnosis);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Diagnosis {0} has been saved.", diagnosis.Code)
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
                    Message = "You must supply a diagnosisId in order to remove the diagnosis."
                });
            }

            var diagnosis = QueryDiagnosis.Load(id.Value);
            if (diagnosis != null)
            {
                DeleteCommand.Execute(diagnosis);
            }

            return Json(
                new JsonActionResponse
                {
                    Status = "Success",
                    Message = String.Format("Diagnosis with code {0} was removed.", diagnosis.Code)
                });
        }

        [HttpGet]
        public JsonResult GetDiagnosis(DiagnosisIndexModel indexModel)
        {
            LoadUserAndClient();

            var pageSize = indexModel.limit.Value;
            var diagnosisDataQuery = this.QueryDiagnosis.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Diagnosis>>>()
            {
                { "Code-ASC", () => diagnosisDataQuery.OrderBy(c => c.Code) },
                { "Code-DESC", () => diagnosisDataQuery.OrderByDescending(c => c.Code) },
                { "ServiceNeeded-ASC", () => diagnosisDataQuery.OrderBy(c => c.ServiceNeeded) },
                { "ServiceNeeded-DESC", () => diagnosisDataQuery.OrderByDescending(c => c.ServiceNeeded) },
                { "Description-ASC", () => diagnosisDataQuery.OrderBy(c => c.Description) },
                { "Description-DESC", () => diagnosisDataQuery.OrderByDescending(c => c.Description) }
            };

            diagnosisDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();
            diagnosisDataQuery = diagnosisDataQuery.Where(it => it.Client.Id == this._client.Id);

            if (!string.IsNullOrEmpty(indexModel.searchValue))
                diagnosisDataQuery = diagnosisDataQuery.Where(it => it.Code.Contains(indexModel.searchValue));

            var totalItems = diagnosisDataQuery.Count();
                
            diagnosisDataQuery = diagnosisDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var diagnosisModelListProjection = (from diagnosis in diagnosisDataQuery.ToList()
                                             select new DiagnosisModel
                                             {
                                                 Id = diagnosis.Id,
                                                 ServiceNeeded = diagnosis.ServiceNeeded,
                                                 Code = diagnosis.Code,
                                                 Description = diagnosis.Description
                                             }).ToArray();


            return Json(new DiagnosisIndexOuputModel
            {
                Diagnosis = diagnosisModelListProjection,
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
            Mapper.CreateMap<DiagnosisInputModel, Diagnosis>();
            Mapper.CreateMap<Diagnosis, DiagnosisInputModel>();
        }
    }
}
