using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.Shared;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Security;
using Core.Security;
using Web.Areas.ConditionManagement.Models.Appointment;
using AutoMapper;

namespace Web.Areas.ConditionManagement.Controllers
{
    public class AppointmentController : Controller
    {
        public IQueryService<Appointment> QueryAppointment { get; set; }
        public ISaveOrUpdateCommand<Appointment> SaveOrUpdateCommand { get; set; }
        public IDeleteCommand<Appointment> DeleteCommand { get; set; }

        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        private Client _client;
        private User _user;

        public IPermissionsService PermissionService { get; set; }
        private string ADD_PERMISSION = "Appointment.Edit";
        private string DELETE_PERMISSION = "Appointment.Delete";

        [HttpGet]
        [Requires(Permissions = "Appointment.View")]
        public ActionResult Overview()
        {
            ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            ViewBag.HasNoRightsToDelete = (PermissionService.HasPermissionAssigned(DELETE_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();

            return View("Overview");
        }

        [HttpPost]
        public JsonResult Create(AppointmentModel appointmentInputModel)
        {
            LoadUserAndClient();

            if (string.IsNullOrEmpty(appointmentInputModel.Code))
            {
                return Json(
                   new JsonActionResponse
                   {
                       Status = "Error",
                       Message = "The appointment has not been saved!"
                   });
            }

            var queryAppointmentValidation = QueryAppointment.Query().Where(p => p.Client == _client);
            if (queryAppointmentValidation.Where(it => it.Code == appointmentInputModel.Code && it.Keyword == appointmentInputModel.Keyword).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a appointment with code {0} and keyword {1}", appointmentInputModel.Code, appointmentInputModel.Keyword)
                    });

            }

            var appointment = new Appointment();
            CreateMappings();
            Mapper.Map(appointmentInputModel, appointment);
            appointment.Client = this._client;
            appointment.ByUser = this._user;

            SaveOrUpdateCommand.Execute(appointment);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Appointment {0} has been saved.", appointment.Code)
               });
        }

        [HttpPost]
        public JsonResult Edit(AppointmentModel appointmentInputModel)
        {
            LoadUserAndClient();

            if (appointmentInputModel.Id == Guid.Empty)
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "You must supply a appointmentId in order to edit the appointment."
                   });
            }

            if (string.IsNullOrEmpty(appointmentInputModel.Code))
            {
                return Json(
                   new ToModalJsonActionResponse
                   {
                       Status = "Error",
                       CloseModal = true,
                       Message = "The appointment has not been saved!"
                   });
            }

            var queryAppointmentValidation = QueryAppointment.Query().Where(p => p.Client == _client);

            if (queryAppointmentValidation.Where(it =>
                                                    it.Code == appointmentInputModel.Code &&
                                                    it.Keyword == appointmentInputModel.Keyword &&
                                                    it.Id != appointmentInputModel.Id).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already a appointment with code {0} and keyword {1}", appointmentInputModel.Code, appointmentInputModel.Keyword)
                    });

            }

            var appointment = QueryAppointment.Load(appointmentInputModel.Id);
            CreateMappings();
            Mapper.Map(appointmentInputModel, appointment);
            appointment.Client = this._client;
            appointment.ByUser = this._user;

            SaveOrUpdateCommand.Execute(appointment);

            return Json(
               new ToModalJsonActionResponse
               {
                   Status = "Success",
                   CloseModal = true,
                   Message = String.Format("Appointment {0} has been saved.", appointment.Code)
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
                    Message = "You must supply a appointmentId in order to remove the appointment."
                });
            }

            var appointment = QueryAppointment.Load(id.Value);
            if (appointment != null)
            {
                DeleteCommand.Execute(appointment);
            }

            return Json(
                new JsonActionResponse
                {
                    Status = "Success",
                    Message = String.Format("Appointment with code {0} was removed.", appointment.Code)
                });
        }

        [HttpGet]
        public JsonResult GetAppointment(AppointmentIndexModel indexModel)
        {
            LoadUserAndClient();

            var pageSize = indexModel.limit.Value;
            var appointmentDataQuery = this.QueryAppointment.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Appointment>>>()
            {
                { "Code-ASC", () => appointmentDataQuery.OrderBy(c => c.Code) },
                { "Code-DESC", () => appointmentDataQuery.OrderByDescending(c => c.Code) },
                { "Keyword-ASC", () => appointmentDataQuery.OrderBy(c => c.Keyword) },
                { "Keyword-DESC", () => appointmentDataQuery.OrderByDescending(c => c.Keyword) },
                { "Description-ASC", () => appointmentDataQuery.OrderBy(c => c.Description) },
                { "Description-DESC", () => appointmentDataQuery.OrderByDescending(c => c.Description) }
            };

            appointmentDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();
            appointmentDataQuery = appointmentDataQuery.Where(it => it.Client.Id == this._client.Id);

            if (!string.IsNullOrEmpty(indexModel.searchValue))
                appointmentDataQuery = appointmentDataQuery.Where(it => it.Code.Contains(indexModel.searchValue));

            var totalItems = appointmentDataQuery.Count();

            appointmentDataQuery = appointmentDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var appointmentModelListProjection = (from appointment in appointmentDataQuery.ToList()
                                                select new AppointmentModel
                                                {
                                                    Id = appointment.Id,
                                                    Keyword = appointment.Keyword,
                                                    Code = appointment.Code,
                                                    Description = appointment.Description
                                                }).ToArray();


            return Json(new AppointmentIndexOuputModel
            {
                Appointment = appointmentModelListProjection,
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
            Mapper.CreateMap<AppointmentModel, Appointment>();
            Mapper.CreateMap<Appointment, AppointmentModel>();
        }

    }
}
