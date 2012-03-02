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
using MileageStats.Web.Models;
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
            var fillup = Using<GetFillupById>().Execute(id);
            
            return new ContentTypeAwareResult(new FillupViewModel(fillup));
        }

        public ActionResult List(int vehicleId)
        {
            var model = GetFillups(vehicleId, null);

            var groups = from fillup in model
                         let year = DateTime.Parse(fillup.Date).Year
                         let month = DateTime.Parse(fillup.Date).ToString("MMMM")
                         let ym = new Tuple<int,string>(year, month)
                         group fillup by ym into grouping
                         select new FillupListViewModel
                                    {
                                        Year = grouping.Key.Item1,
                                        Month = grouping.Key.Item2, 
                                        Fillups = grouping
                                    };

            return new ContentTypeAwareResult(groups.ToList())
            {
                WhenJson = (m,v) => Json(m, JsonRequestBehavior.AllowGet),
                WhenHtml = (m,v) => View(m)
            };
        }

        public ActionResult ListPartial(int vehicleId)
        {
            var vehicle = Using<GetVehicleById>()
                .Execute(CurrentUserId, vehicleId);

            if (vehicle == null)
                throw new HttpException((int)HttpStatusCode.NotFound, Messages.FillupController_VehicleNotFound);

            var fillups = Using<GetFillupsForVehicle>()
                .Execute(vehicleId)
                .Select(m => new FillupViewModel(m))
                .OrderByDescending(f => f.Date);

            return PartialView(fillups.ToList());
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

            return new ContentTypeAwareResult(model);
        }

        private IEnumerable<FillupViewModel> GetFillups(int vehicleId, int? selectedFillup)
        {
            var vehicle = Using<GetVehicleById>()
                .Execute(CurrentUserId, vehicleId);

            if (vehicle == null)
                throw new HttpException((int)HttpStatusCode.NotFound, Messages.FillupController_VehicleNotFound);

            var fillups = Using<GetFillupsForVehicle>()
                .Execute(vehicleId)
                .OrderByDescending(f => f.Date);

            var model = fillups.Select(m => new FillupViewModel(m)).ToList();

            if (selectedFillup.HasValue)
            {
                ViewBag.SelectedFillup = model.FirstOrDefault(f => f.FillupEntryId == selectedFillup.Value);
            }

            return model;
        }

    }

    //public class VehicleContextFilterAttribute:IFilterProvider
    //{
    //    public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
    //    {
    //        int vehicleId = controllerContext.RequestContext.RouteData[];
    //        var controller = (BaseController) controllerContext.Controller;
    //        var viewbag = controllerContext.Controller.ViewBag;
    //        viewbag.VehicleName = controller.GetVehicleName(vehicleId);
    //        viewbag.VehicleId = vehicleId;
    //    }
    //}
}