/*  
Copyright Microsoft Corporation

Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at 

http://www.apache.org/licenses/LICENSE-2.0 

THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
ARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
MERCHANTABLITY OR NON-INFRINGEMENT. 

See the Apache 2 License for the specific language governing permissions and
limitations under the License. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using MileageStats.Web.Models;
using System.Net;
using MileageStats.Web.Properties;

namespace MileageStats.Web.Controllers
{
    public class VehicleController : BaseController
    {
        public VehicleController(
            GetUserByClaimId getUser,
            IServiceLocator serviceLocator)
            : base(getUser, serviceLocator)
        {
        }

        [Authorize]
        public ActionResult Details(int id)
        {
            var vehicles = Using<GetVehicleListForUser>()
                .Execute(this.CurrentUserId);

            var selected = vehicles
                .FirstOrDefault(x => x.VehicleId == id);

            if (selected == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound,
                    Messages.VehicleController_VehicleNotFound);
            }

            // we are limiting this to 3 reminders 
            // after we retrieve the full set from the server
            var overdue = Using<GetOverdueRemindersForVehicle>()
                .Execute(id, DateTime.UtcNow, selected.Odometer ?? 0)
                .Take(3);

            var vm = new VehicleDetailsViewModel
                         {
                             VehicleList = new VehicleListViewModel(vehicles, id) { IsCollapsed = true },
                             Vehicle = selected,
                             OverdueReminders = overdue,
                             UserId = CurrentUserId
                         };
            vm.VehicleList.IsCollapsed = true;

            return new ContentTypeAwareResult(vm);
        }

        private ViewResult SetupVehicleForm(VehicleFormModel vehicleForm)
        {
            var handler = Using<GetYearsMakesAndModels>();
            var yearsMakesAndModels = handler.Execute(vehicleForm.Year, vehicleForm.MakeName);

            ViewBag.Years = EnumerableToSelectList(yearsMakesAndModels.Item1, vehicleForm.Year);
            ViewBag.Makes = EnumerableToSelectList(yearsMakesAndModels.Item2, vehicleForm.MakeName);
            ViewBag.Models = EnumerableToSelectList(yearsMakesAndModels.Item3, vehicleForm.ModelName);

            return View(vehicleForm);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Add()
        {
            return SetupVehicleForm(new VehicleFormModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize]
        public ActionResult Add(VehicleFormModel vehicleForm, HttpPostedFileBase photoFile, string action)
        {
            if (!string.IsNullOrEmpty(action) && action.Equals("Save") && ModelState.IsValid)
            {
                int vehicleId;

                if (TrySaveVehicle(vehicleForm, photoFile, out vehicleId))
                {
                    this.SetConfirmationMessage(Messages.VehicleController_VehicleAdded);

                    return RedirectToAction("Details", "Vehicle", new { id = vehicleId });
                }
            }

            vehicleForm.Action = action;
            return SetupVehicleForm(vehicleForm);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var vehicleForm = GetVehicleForm(id);

            return SetupVehicleForm(vehicleForm);
        }

        private VehicleFormModel GetVehicleForm(int vehicleId)
        {
            var vehicles = Using<GetVehicleListForUser>()
                .Execute(CurrentUserId);

            var selected = vehicles
                .FirstOrDefault(x => x.VehicleId == vehicleId);

            if (selected == null)
            {
                throw new HttpException((int) HttpStatusCode.NotFound,
                                        Messages.VehicleController_VehicleNotFound);
            }

            var vehicleForm = new VehicleFormModel
                                  {
                                      VehicleId = selected.VehicleId,
                                      Name = selected.Name,
                                      Year = selected.Year,
                                      MakeName = selected.MakeName,
                                      ModelName = selected.ModelName,
                                      SortOrder = selected.SortOrder
                                  };
            return vehicleForm;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(VehicleFormModel vehicleForm, HttpPostedFileBase photoFile, string action)
        {
            if (!string.IsNullOrEmpty(action) && action.Equals("Save") && ModelState.IsValid)
            {
                if (TryUpdateVehicle(vehicleForm, photoFile))
                {
                    this.SetConfirmationMessage(Messages.VehicleController_VehicleUpdated);

                    return RedirectToAction("Details", "Vehicle", new {id = vehicleForm.VehicleId});
                }
            }

            return SetupVehicleForm(vehicleForm);
        }

        public ActionResult ConfirmDelete(int id)
        {
            var vehicleForm = GetVehicleForm(id);
            return View(vehicleForm);    
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            Using<DeleteVehicle>().Execute(CurrentUserId, id);

            this.SetConfirmationMessage(Messages.VehicleController_VehicleDeleted);

            return new ContentTypeAwareResult
            {
                WhenHtml = (m, v) => RedirectToAction("Index", "Dashboard"),
                WhenJson = (m, v) => new HttpStatusCodeResult((int)HttpStatusCode.OK, Messages.VehicleController_VehicleDeleted)
            };
        }

        [Authorize]
        [ChildActionOnly]
        public ActionResult ListPartial(int? vehicleId, bool isCollapsed)
        {
            var vehicles = Using<GetVehicleListForUser>()
                    .Execute(CurrentUserId);

            var viewModel = new VehicleListViewModel(vehicles, vehicleId.GetValueOrDefault()) {IsCollapsed = isCollapsed};

            return PartialView(viewModel);
        }

        private static SelectList EnumerableToSelectList<T>(IEnumerable<T> source, object selectValue)
        {
            return new SelectList(source.Select(x => new {Value = x.ToString(), Text = x.ToString()}), "Value", "Text",
                                  selectValue);
        }

        private bool TrySaveVehicle(VehicleFormModel vehicleForm, HttpPostedFileBase photoFile, out int vehicleId)
        {
            vehicleId = -1;

            IEnumerable<ValidationResult> vehicleErrors = Using<CanAddVehicle>().Execute(CurrentUserId,
                vehicleForm);

            ModelState.AddModelErrors(vehicleErrors, "Save");

            if (!ModelState.IsValid) return false;

            ValidatePostedPhotoFile(photoFile);

            if (ModelState.IsValid)
            {
                vehicleId =  Using<CreateVehicle>().Execute(CurrentUserId, vehicleForm, photoFile);
                return true;
            }
            return false;
        }

        private bool TryUpdateVehicle(VehicleFormModel vehicleForm, HttpPostedFileBase photoFile)
        {
            IEnumerable<ValidationResult> vehicleErrors =
                Using<CanValidateVehicleYearMakeAndModel>().Execute(vehicleForm);
            ModelState.AddModelErrors(vehicleErrors, "Edit");

            if (!ModelState.IsValid) return false;

            ValidatePostedPhotoFile(photoFile);

            if (ModelState.IsValid)
            {
                Using<UpdateVehicle>().Execute(CurrentUserId, vehicleForm, photoFile);
                return true;
            }
            return false;
        }

        private void ValidatePostedPhotoFile(HttpPostedFileBase photoFile)
        {
            if (photoFile == null) return; // the photo is optional, so no validation errors if it's omitted

            IEnumerable<ValidationResult> photoErrors =
                Using<CanAddPhoto>().Execute(photoFile.InputStream,
                                             photoFile.ContentLength,
                                             photoFile.ContentType);

            ModelState.AddModelErrors(photoErrors, "photoFile");
        }

        #region JSON endpoints

        // All JSON endpoints require [HttpPost] to prevent JSON hijacking.
        // With [HttpPost], returning arrays is allowed.  
        // See http://haacked.com/archive/2009/06/25/json-hijacking.aspx

        [HttpPost]
        [Authorize]
        public JsonResult JsonList()
        {
            var list = Using<GetVehicleListForUser>()
                .Execute(CurrentUserId)
                .Select(x => ToJsonVehicleViewModel(x))
                .ToList();

            return Json(list);
        }

        [HttpPost]
        [Authorize]
        public JsonResult JsonDetails(int id)
        {
            var vehicle = Using<GetVehicleById>()
                .Execute(CurrentUserId, vehicleId: id);

            // we are limiting this to 3 reminders 
            // after we retrieve the full set from the server
            var overdue = Using<GetOverdueRemindersForVehicle>()
                .Execute(id, DateTime.UtcNow, vehicle.Odometer ?? 0)
                .Take(3);

            var vm = ToJsonVehicleViewModel(vehicle, overdue);

            return Json(vm);
        }

        [HttpPost]
        [Authorize]
        public void UpdateSortOrder(UpdateVehicleSortOrderViewModel newVehicleListOrder)
        {
            Using<UpdateVehicleSortOrder>().Execute(CurrentUserId, newVehicleListOrder.VehicleSortOrder);
        }

        private static JsonVehicleViewModel ToJsonVehicleViewModel(VehicleModel vehicle,
                                                                   IEnumerable<ReminderSummaryModel> overdue = null)
        {
            JsonStatisticsViewModel last12Stats = ToJsonStatisticsViewModel(vehicle.Statistics);

            return new JsonVehicleViewModel
                       {
                           VehicleId = vehicle.VehicleId,
                           Name = vehicle.Name,
                           SortOrder = vehicle.SortOrder,
                           Year = vehicle.Year,
                           MakeName = vehicle.MakeName,
                           ModelName = vehicle.ModelName,
                           Odometer = vehicle.Odometer,
                           PhotoId = vehicle.PhotoId,
                           LifeTimeStatistics = new JsonStatisticsViewModel(),
                           //not used
                           LastTwelveMonthsStatistics = last12Stats,
                           OverdueReminders = overdue ?? new List<ReminderSummaryModel>()
                       };
        }

        private static JsonStatisticsViewModel ToJsonStatisticsViewModel(VehicleStatisticsModel statistics)
        {
            return new JsonStatisticsViewModel
                       {
                           Name = statistics.Name,
                           AverageFillupPrice = statistics.AverageFillupPrice,
                           AverageFuelEfficiency = statistics.AverageFuelEfficiency,
                           AverageCostPerMonth = statistics.AverageCostPerMonth,
                           AverageCostToDrive = statistics.AverageCostToDrive,
                           Odometer = statistics.Odometer,
                           TotalDistance = statistics.TotalDistance,
                           TotalFuelCost = statistics.TotalFuelCost,
                           TotalUnits = statistics.TotalUnits,
                           TotalCost = statistics.TotalCost
                       };
        }

        #endregion
    }
}