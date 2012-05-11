using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Areas.ServicesManagement.Models.Treatment;
using Web.Models.Shared;
using AutoMapper;

namespace Web.Areas.ServicesManagement.Controllers
{
    public class TreatmentController : Controller
    {
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        public IQueryService<Treatment> QueryTreatments { get; set; }

        public ISaveOrUpdateCommand<Treatment> SaveOrUpdateCommand { get; set; }
        public IDeleteCommand<Treatment> DeleteCommand { get; set; }

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
        public JsonResult Create(TreatmentInputModel treatmentInputModel)
        {
            LoadUserAndClient();

            if (string.IsNullOrEmpty(treatmentInputModel.Code))
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "The treatment has not been saved!"
                   });
            }

            var queryTreatmentValidation = QueryTreatments.Query().Where(p => p.Client == _client);
            if (queryTreatmentValidation.Where(it => it.Code == treatmentInputModel.Code && it.Advice == treatmentInputModel.Advice).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a treatment with code {0} and advice {1}", treatmentInputModel.Code, treatmentInputModel.Advice)
                    });

            }

            var treatment = new Treatment();
            CreateMappings();
            Mapper.Map(treatmentInputModel, treatment);
            treatment.Client = this._client;
            treatment.ByUser = this._user;

            SaveOrUpdateCommand.Execute(treatment);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Treatment {0} has been saved.", treatment.Code)
               });
        }

        [HttpPost]
        public JsonResult Edit(TreatmentInputModel treatmentInputModel)
        {
            LoadUserAndClient();

            if (treatmentInputModel.Id == Guid.Empty)
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "You must supply a treatmentId in order to edit the treatment."
                   });
            }

            if (string.IsNullOrEmpty(treatmentInputModel.Code))
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "The treatment has not been saved!"
                   });
            }

            var queryTreatmentValidation = QueryTreatments.Query().Where(p => p.Client == _client);

            if (queryTreatmentValidation.Where(it =>
                                                    it.Code == treatmentInputModel.Code &&
                                                    it.Advice == treatmentInputModel.Advice &&
                                                    it.Id != treatmentInputModel.Id).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a treatment with code {0} and advice {1}", treatmentInputModel.Code, treatmentInputModel.Advice)
                    });

            }

            var treatment = new Treatment();
            CreateMappings();
            Mapper.Map(treatmentInputModel, treatment);
            treatment.Client = this._client;
            treatment.ByUser = this._user;

            SaveOrUpdateCommand.Execute(treatment);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Treatment {0} has been saved.", treatment.Code)
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
                    Message = "You must supply a treatmentId in order to remove the treatment."
                });
            }

            var treatment = QueryTreatments.Load(id.Value);
            if (treatment != null)
            {
                DeleteCommand.Execute(treatment);
            }

            return Json(
                new JsonActionResponse
                {
                    Status = "Success",
                    Message = String.Format("Treatment with code {0} was removed.", treatment.Code)
                });
        }

        [HttpGet]
        public JsonResult GetTreatments(TreatmentIndexModel indexModel)
        {
            LoadUserAndClient();

            var pageSize = indexModel.limit.Value;
            var treatmentDataQuery = QueryTreatments.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Treatment>>>()
            {
                { "Code-ASC", () => treatmentDataQuery.OrderBy(c => c.Code) },
                { "Code-DESC", () => treatmentDataQuery.OrderByDescending(c => c.Code) },
                { "Advice-ASC", () => treatmentDataQuery.OrderBy(c => c.Advice) },
                { "Advice-DESC", () => treatmentDataQuery.OrderByDescending(c => c.Advice) },
                { "Description-ASC", () => treatmentDataQuery.OrderBy(c => c.Description) },
                { "Description-DESC", () => treatmentDataQuery.OrderByDescending(c => c.Description) }
            };

            treatmentDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();
            treatmentDataQuery = treatmentDataQuery.Where(it => it.Client.Id == this._client.Id);

            if (!string.IsNullOrEmpty(indexModel.searchValue))
                treatmentDataQuery = treatmentDataQuery.Where(it => it.Code.Contains(indexModel.searchValue));

            var totalItems = treatmentDataQuery.Count();

            treatmentDataQuery = treatmentDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var treatmentModelListProjection = (from treatment in treatmentDataQuery.ToList()
                                                select new TreatmentModel
                                                {
                                                    Id = treatment.Id,
                                                    Advice = treatment.Advice,
                                                    Code = treatment.Code,
                                                    Description = treatment.Description
                                                }).ToArray();


            return Json(new TreatmentIndexOuputModel
            {
                Treatments = treatmentModelListProjection,
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
            Mapper.CreateMap<TreatmentInputModel, Treatment>();
            Mapper.CreateMap<Treatment, TreatmentInputModel>();
        }

    }
}
