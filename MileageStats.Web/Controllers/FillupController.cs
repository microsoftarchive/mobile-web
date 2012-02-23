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
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using MileageStats.Web.Models;
using MileageStats.Domain.Properties;
using MileageStats.Web.Properties;
using System.Web;
using System.Net;

namespace MileageStats.Web.Controllers
{
    [Authorize]
    public class FillupController : BaseController
    {
        public FillupController(GetUserByClaimId getUser, IServiceLocator serviceLocator)
            : base(getUser, serviceLocator)
        {
        }

        public ActionResult Details(int vehicleId, int id)
        {
            return List(vehicleId, id);
        }

        public ActionResult List(int vehicleId, int? selectedFillup = null)
        {
            ViewBag.VehicleName = GetVehicleName(vehicleId);
            ViewBag.VehicleId = vehicleId;

            var model = GetFillups(vehicleId, selectedFillup);

            return new ContentTypeAwareResult(model)
            {
                WhenJson = (m,v) => Json(m, JsonRequestBehavior.AllowGet),
                WhenHtml = (m,v) => View(m)
            };
        }

        public ActionResult ListPartial(int vehicleId)
        {
            var model = GetFillups(vehicleId, null);
            
            return PartialView(model);
        }

        public ActionResult Add(int vehicleId)
        {
            var vehicle = Using<GetVehicleById>()
                .Execute(CurrentUserId, vehicleId);

            if (vehicle == null)
                throw new HttpException((int)HttpStatusCode.NotFound,
                    Messages.FillupController_VehicleNotFound);

            var fillups = Using<GetFillupsForVehicle>()
                .Execute(vehicleId)
                .OrderByDescending(f => f.Date);

            var newFillupEntry = new FillupEntryFormModel
                                        {
                                            Odometer = vehicle.Odometer.HasValue ? vehicle.Odometer.Value : 0,
                                            VehicleId = vehicleId
                                        };

            ViewBag.IsFirstFillup = (!fillups.Any());

            return View(newFillupEntry); 
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int vehicleId, FillupEntryFormModel model)
        {
            // Vehicle does not need to validate here as we are using a forgery token

            if (ModelState.IsValid)
            {
                var errors = Using<CanAddFillup>()
                    .Execute(CurrentUserId, vehicleId, model);

                if (errors.Any())
                {
                    ModelState.AddModelErrors(errors, "AddFillup");
                }
                else
                {
                    Using<AddFillupToVehicle>().Execute(CurrentUserId, vehicleId, model);

                    this.SetConfirmationMessage(Messages.FillupController_FillupAddedMessage);
                    
                    return RedirectToAction("Details", "Vehicle", new { Id = vehicleId });
                }
            }

            var fillups = Using<GetFillupsForVehicle>()
                .Execute(vehicleId)
                .OrderByDescending(f => f.Date);

            ViewBag.IsFirstFillup = (!fillups.Any());

            return View(model);
        }

        public static List<FillupViewModel> ToJsonFillupViewModel(IEnumerable<FillupEntry> fillupEntries)
        {
            return fillupEntries.Select(entry => new FillupViewModel
            {
                FillupEntryId = entry.FillupEntryId,
                Date = String.Format("{0:d MMM yyyy}", entry.Date),
                TotalUnits = String.Format("{0:#00.000}", entry.TotalUnits),
                Odometer = entry.Odometer,
                TransactionFee = String.Format("{0:C}", entry.TransactionFee),
                PricePerUnit = String.Format("{0:0.000}", entry.PricePerUnit),
                Remarks = entry.Remarks,
                Vendor = entry.Vendor,
                TotalCost = String.Format("{0:C}", entry.TotalCost)
            }).ToList();
        }

        private List<FillupViewModel> GetFillups(int vehicleId, int? selectedFillup)
        {
            var vehicle = Using<GetVehicleById>()
                .Execute(CurrentUserId, vehicleId);

            if (vehicle == null)
                throw new HttpException((int)HttpStatusCode.NotFound, Messages.FillupController_VehicleNotFound);

            var fillups = Using<GetFillupsForVehicle>()
                .Execute(vehicleId)
                .OrderByDescending(f => f.Date);

            var model = ToJsonFillupViewModel(fillups);

            if (selectedFillup.HasValue)
            {
                ViewBag.SelectedFillup = model.FirstOrDefault(f => f.FillupEntryId == selectedFillup.Value);
            }

            return model;
        }

        
    }
}