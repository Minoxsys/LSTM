﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.OutpostManagement.Models.Contact;
using AutoMapper;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Models.Shared;
using Web.Security;
using Core.Security;

namespace Web.Areas.OutpostManagement.Controllers
{
	public class OutpostController : Controller
	{
		public OutpostModel OutpostModel { get; set; }
		public OutpostOutputModel OutpostModelOutput { get; set; }

		public IQueryService<Outpost> QueryWarehouse { get; set; }
		public IQueryService<Outpost> QueryService { get; set; }
		public IQueryService<Country> QueryCountry { get; set; }
		public IQueryService<Region> QueryRegion { get; set; }
		public IQueryService<District> QueryDistrict { get; set; }
		public IQueryService<Client> LoadClient { get; set; }
		public IQueryService<User> QueryUsers { get; set; }
		public IQueryService<Contact> QueryContact { get; set; }
        public IQueryService<OutpostType> QueryOutpostTypes { get; set; }

		public ISaveOrUpdateCommand<Outpost> SaveOrUpdateCommand { get; set; }
		public ISaveOrUpdateCommand<Contact> SaveOrUpdateCommandContact { get; set; }

		public IDeleteCommand<Outpost> DeleteCommand { get; set; }
		public IDeleteCommand<Contact> DeleteContactCommand { get; set; }

		public OutpostOutputModel OutpostOutputModel { get; set; }

		public OutpostInputModel OutpostInputModel { get; set; }

		public OutpostOutputModel CreateOutpost { get; set; }

        public IPermissionsService PermissionService { get; set; }

        private const String OUTPOST_ADD_PERMISSION = "Outpost.Edit";
        private const String OUTPOST_DELETE_PERMISSION = "Outpost.Delete";

		private const string TEMPDATA_ERROR_KEY = "error";
		private Core.Domain.User _user;
		private Client _client;


        [HttpGet]
        [Requires(Permissions = "Outpost.View")]
        public ActionResult Overview()
        {
            ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(OUTPOST_ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            ViewBag.HasNoRightsToDelete = (PermissionService.HasPermissionAssigned(OUTPOST_DELETE_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();           
            
            Guid? districtId = (Guid?)TempData["FromDistrictsId"];

            OutpostOverviewModel model = new OutpostOverviewModel();
            if (districtId.HasValue)
            {
                var district = QueryDistrict.Load(districtId.Value);
                model.DistrictId = district.Id;
                model.RegionId = district.Region.Id;
                model.CountryId = district.Region.Country.Id;
            }

            return View("Overview", model);
        }

        [HttpGet]
        public ActionResult FromDistricts(Guid? districtId)
        {
            if (districtId.HasValue)
            {
                TempData.Clear();
                TempData.Add("FromDistrictsId", districtId.Value);
            }

            return RedirectToAction("Overview", "Outpost");
        }

		[HttpPost]
		public JsonResult Delete(Guid outpostId)
		{
			var outpost = QueryService.Load(outpostId);

            var contacts = QueryContact.Query().Where(it => it.Outpost == outpost);

            foreach (var contact in contacts)
            {
                DeleteContactCommand.Execute(contact);
            }

			if (outpost != null)
			{
				DeleteCommand.Execute(outpost);
			}

			return Json(new JsonActionResponse
			{
				Status = "Success",
				Message = string.Format("Successfully removed health facility {0}", outpost.Name)
			});
		}

		public JsonResult GetOutposts(GetOutpostsInputModel input)
		{
			var model = new GetOutpostsOutputModel();
			LoadUserAndClient();

			var outpostsQueryData = QueryService.Query().
			Where(c => c.Client == this._client);

			if (input.countryId.HasValue)
			{
				outpostsQueryData = outpostsQueryData.Where(o => o.Country.Id == input.countryId.Value);
			}

			if (input.regionId.HasValue)
			{
				outpostsQueryData = outpostsQueryData.Where(o => o.Region.Id == input.regionId.Value);
			}

			if (input.districtId.HasValue)
			{
				outpostsQueryData = outpostsQueryData.Where(o => o.District.Id == input.districtId.Value);
			}

			if (!string.IsNullOrEmpty(input.search))
			{
				outpostsQueryData = outpostsQueryData.Where(o => o.Name.Contains(input.search));
			}

            if (input.outpostTypeId.HasValue)
            {
                outpostsQueryData = outpostsQueryData.Where( o => o.OutpostType.Id == input.outpostTypeId.Value);
            }

			var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Outpost>>>()
			{
				{ "Name-ASC", () => outpostsQueryData.OrderBy(c => c.Name) },
				{ "Name-DESC", () => outpostsQueryData.OrderByDescending(c => c.Name) }
			};

			Func<IQueryable<Outpost>> orderOutposts;
			if (orderByColumnDirection.TryGetValue(String.Format("{0}-{1}", input.sort, input.dir), out orderOutposts))
			{
				outpostsQueryData = orderOutposts.Invoke();
			}

			model.TotalItems = outpostsQueryData.Count();
            
			outpostsQueryData = outpostsQueryData.Take(input.limit.Value).Skip(input.start.Value);
			model.Outposts = (from o in outpostsQueryData.ToList() select new GetOutpostsOutputModel.OutpostModel
			{
				Id = o.Id.ToString(),
				Name = o.Name,
				WarehouseName = o.Warehouse != null ? o.Warehouse.Name : string.Empty,
				Coordinates = o.Latitude + "" + o.Longitude, // TODO drop this and just add a simple Coordintates property, the client should have accepted it in the first place
				ContactMethod = o.DetailMethod,
				CountryId = o.Country.Id.ToString(),
				RegionId = o.Region.Id.ToString(),
				DistrictId = o.District.Id.ToString(),
                OutpostTypeId = o.OutpostType.Id.ToString(),
                OutpostTypeName = o.OutpostType.Name,
				WarehouseId = o.Warehouse != null ? o.Warehouse.Id.ToString() : string.Empty
			}).ToArray();

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetDistricts(Guid? regionId)
		{
			LoadUserAndClient();
			var model = new GetDistrictsOutputModel();

			if (regionId.HasValue && regionId.Value != Guid.Empty)
			{
				model.Districts = this.QueryDistrict.Query().Where(m => m.Region.Id == regionId &&
																		m.Client == _client).
				Select(district => new GetDistrictsOutputModel.DistrictModel
				{
					Id = district.Id,
					Name = district.Name
				}).ToArray();
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

        [HttpGet]
        public JsonResult GetOutpostTypes()
        {
            LoadUserAndClient();

            var outpostTypes = QueryOutpostTypes.Query();
            int totalItems = outpostTypes.Count();

            var outpostTypesModelListProjection = (from type in outpostTypes.ToList()
                                              select new OutpostTypeModel
                                              {
                                                  Id = type.Id,
                                                  Name = type.Name,
                                                  Type = type.Type
                                              }).ToArray();


            return Json(new OutpostTypesIndexOutputModel
            {
                OutpostTypes = outpostTypesModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWarehouses()
        {
            var model = new GetWarehousesOutputModel();
            LoadUserAndClient();

            var warehouseQueryData = QueryService.Query()
                                                 .Where(c => c.Client == this._client && c.OutpostType.Name != "Drug shop")
                                                 .OrderBy(w => w.Name);
            model.Warehouses = (from w in warehouseQueryData.ToList()
                                select new GetWarehousesOutputModel.WarehouseModel
                                    {
                                        Id = w.Id.ToString(),
                                        Name = w.Name
                                    }).ToArray();

            return Json(model, JsonRequestBehavior.AllowGet);
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

			this._client = LoadClient.Load(clientId);
		}

		public class OutpostCreateResponse : JsonActionResponse
		{
			public Guid OutpostId { get; set; }
		}

		[HttpPost]
		public JsonResult Create(CreateOutpostInputModel model)
		{
			LoadUserAndClient();

            var queryoutpost = QueryService.Query().Where(p => p.Client == _client);
            if (queryoutpost.Where(it => it.Name == model.Name && it.District.Id == model.DistrictId).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already an health facility with this name: {0} for this district! Please insert a different name!", model.Name)
                    });

            }

            if (queryoutpost.Where(it => it.Latitude == model.Coordinates).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already an health facility with this coordinates: {0}. Please choose different coordinates!", model.Coordinates)
                    });
 
            }
            
			var outpost = new Outpost();
			MapInputToOutpost(model, ref outpost);

			SaveOrUpdateCommand.Execute(outpost);

			return Json(new OutpostCreateResponse
			{
                Message = string.Format("Created successfully health facility {0}", outpost.Name),
				OutpostId= outpost.Id,
				Status = "Success"
			});
		}

		[HttpPost]
		public JsonResult Edit(EditOutpostInputModel model)
		{
			LoadUserAndClient();

            var queryoutpost = QueryService.Query().Where(p => p.Client == _client);
            if (queryoutpost.Where(it => it.Name == model.Name && it.District.Id == model.DistrictId && it.Id != model.EntityId.Value).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already an health facility with this name: {0} for this district! Please insert a different name!", model.Name)
                    });

            }

            if (queryoutpost.Where(it => it.Latitude == model.Coordinates && it.Id != model.EntityId.Value).Count() > 0)
            {
                return Json(
                    new ToModalJsonActionResponse
                    {
                        Status = "Error",
                        CloseModal = false,
                        Message = string.Format("There is already an health facility with this coordinates: {0}. Please choose different coordinates!", model.Coordinates)
                    });

            }
			var outpost = QueryService.Load(model.EntityId.Value);
			MapInputToOutpost(model, ref outpost);

			SaveOrUpdateCommand.Execute(outpost);

			return Json(new JsonActionResponse
			{
                Message = string.Format("Saved successfully health facility {0}", outpost.Name),
				Status = "Success"
			});
		}

		private void MapInputToOutpost(CreateOutpostInputModel model, ref Outpost outpost)
		{
			outpost. Name = model.Name;
            outpost.OutpostType = QueryOutpostTypes.Load(model.OutpostTypeId.Value);
			outpost. Latitude = model.Coordinates;
			outpost. Client = _client;
			outpost. ByUser = _user;

			outpost.Country = QueryCountry.Load(model.CountryId.Value);
			outpost.Region = QueryRegion.Load(model.RegionId.Value);
			outpost.District = QueryDistrict.Load(model.DistrictId.Value);

			outpost.Warehouse = null;
			if (model.WarehouseId.HasValue)
			{
				outpost.Warehouse = QueryService.Load(model.WarehouseId.Value);
			}
		}
	}
}