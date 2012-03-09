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
using MileageStats.Domain.Contracts;
using MileageStats.Domain.Handlers;
using MileageStats.Domain.Models;
using MileageStats.Web.Controllers;
using MileageStats.Web.Models;
using MileageStats.Web.Tests.Mocks;
using Moq;
using Xunit;

namespace MileageStats.Web.Tests.Controllers
{
    public class ReminderControllerFixture
    {
        private const int defaultVehicleId = 99;
        private readonly User _defaultUser;

        private readonly Mock<GetUserByClaimId> _mockUserServices;
        private readonly Mock<IServiceLocator> _serviceLocator;

        public ReminderControllerFixture()
        {
            _serviceLocator = new Mock<IServiceLocator>();

            _defaultUser = new User { AuthorizationId = "TestClaimsIdentifier", UserId = 5 };

            _mockUserServices = new Mock<GetUserByClaimId>(null);
            _mockUserServices
                .Setup(s => s.Execute(_defaultUser.AuthorizationId))
                .Returns(_defaultUser);
        }

        [Fact]
        public void WhenListReminderGetWithValidVehicleId_ThenReturnsView()
        {
            var vehicle = new VehicleModel(new Vehicle {VehicleId = defaultVehicleId},
                                                             new VehicleStatisticsModel());

            var reminders = new[]
                                {
                                   new Reminder {ReminderId = 1},
                                   new Reminder {ReminderId = 2}
                                };

            MockHandlerFor<GetUnfulfilledRemindersForVehicle>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, defaultVehicleId, 0))
                         .Returns(reminders));

            MockHandlerFor<GetVehicleById>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, defaultVehicleId))
                         .Returns(vehicle));

            ReminderController controller = GetTestableReminderController();
            ActionResult result = controller.List(defaultVehicleId);

            var model = result.Extract<ReminderDetailsViewModel>();

            Assert.Equal(reminders.Length, model.Reminders.Count());
            Assert.Equal(reminders[0].ReminderId, model.Reminder.ReminderId);
        }

        [Fact]
        public void WhenAddReminderGetWithValidVehicleId_ThenReturnsView()
        {
            MockDefaultHandlers();
            

            ReminderController controller = GetTestableReminderController();

            ActionResult result = controller.Add(defaultVehicleId);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void WhenAddReminderPostWithNullReminder_ThenReturnsToCreatePage()
        {
            MockDefaultHandlers();

            ReminderController controller = GetTestableReminderController();

            ActionResult result = controller.Add(defaultVehicleId, null);
            
            Assert.IsType(typeof(ViewResult), result);

            var viewResult = result as ViewResult;
            Assert.Equal(MileageStats.Web.Properties.Messages.PleaseFixInvalidData, viewResult.TempData["alert"]); 
        }

        [Fact]
        public void WhenAddReminderPostWithInvalidReminder_ThenReturnsToCreatePage()
        {
            MockDefaultHandlers();

            ReminderController controller = GetTestableReminderController();
            controller.ModelState.AddModelError("test", "test error");

            var reminderForm = new ReminderFormModel();
            ActionResult result = controller.Add(defaultVehicleId, reminderForm);

            Assert.IsType(typeof(ViewResult), result);
        }

        [Fact]
        public void WhenAddRemindersWithValidReminder_ThenUpdatesVehicleInRepository()
        {
            var formModel = new ReminderFormModel();

            MockDefaultHandlers();

            MockHandlerFor<CanAddReminder>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, formModel))
                         .Returns(new ValidationResult[] { }));

            var handler = MockHandlerFor<AddReminderToVehicle>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, defaultVehicleId, formModel))
                         .Verifiable("add handler not called"));

            var controller = GetTestableReminderController();
            controller.Add(defaultVehicleId, formModel);

            handler.Verify();
        }

        [Fact]
        public void WhenAddReminderWithValidReminder_ThenReturnsToReminderListByGroupView()
        {
            var formModel = new ReminderFormModel();

            MockDefaultHandlers();

            MockHandlerFor<CanAddReminder>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, formModel))
                         .Returns(new ValidationResult[] { }));

            MockHandlerFor<AddReminderToVehicle>(
                x => x.Setup(h => h.Execute(_defaultUser.UserId, defaultVehicleId, formModel)));

            ReminderController controller = GetTestableReminderController();
            var result = (RedirectToRouteResult)controller.Add(defaultVehicleId, formModel);

            Assert.NotNull(result);
            Assert.Equal("ListByGroup", result.RouteValues["action"]);
            Assert.Equal("Reminder", result.RouteValues["controller"]);
        }

        [Fact]
        public void WhenOverdueList_ThenReturnsModel()
        {
            ReminderController controller = GetTestableReminderController();

            controller.HttpContext.Request.SetAjaxRequest();
            controller.HttpContext.Request.SetJsonRequest();

            var dueDate = new DateTime(2010, 12, 1, 0, 0, 0, DateTimeKind.Utc);
            var reminders = new[]
                                {
                                    new ReminderModel
                                        {
                                            ReminderId = 1,
                                            VehicleId = defaultVehicleId,
                                            Title = "Reminder1",
                                            DueDate = dueDate
                                        },
                                    new ReminderModel
                                        {
                                            ReminderId = 2,
                                            VehicleId = defaultVehicleId,
                                            Title = "Reminder2",
                                            DueDistance = 1000
                                        },
                                    new ReminderModel
                                        {
                                            ReminderId = 3,
                                            VehicleId = defaultVehicleId,
                                            Title = "Reminder3",
                                            DueDate = dueDate,
                                            DueDistance = 1000
                                        },
                                };

            MockHandlerFor<GetOverdueRemindersForUser>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId))
                         .Returns(reminders));

            MockHandlerFor<GetVehicleById>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, defaultVehicleId))
                         .Returns(new VehicleModel(
                                      new Vehicle { VehicleId = defaultVehicleId, Name = "Vehicle" },
                                      new VehicleStatisticsModel())));

            var result = (JsonResult)controller.OverdueList();
            var model = (JsonRemindersOverdueListViewModel)result.Data;
            var list = new List<OverdueReminderViewModel>(model.Reminders);

            Assert.Equal(reminders.Length, model.Reminders.Count());

            Assert.Equal("Reminder1 | Vehicle @ 12/1/2010", list[0].FullTitle);
            Assert.Equal("Reminder2 | Vehicle @ 1000", list[1].FullTitle);
            Assert.Equal("Reminder3 | Vehicle @ 12/1/2010 or 1000", list[2].FullTitle);
        }

        [Fact]
        public void WhenOverdueListWhenNoReminders_ThenReturnsModelWithEmptyCollection()
        {
            ReminderController controller = GetTestableReminderController();

            controller.HttpContext.Request.SetAjaxRequest();
            controller.HttpContext.Request.SetJsonRequest();

            MockHandlerFor<GetOverdueRemindersForUser>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId))
                         .Returns(new ReminderModel[] { }));

            MockHandlerFor<GetVehicleById>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, defaultVehicleId))
                         .Returns(new VehicleModel(
                                      new Vehicle { VehicleId = defaultVehicleId, Name = "Vehicle" },
                                      new VehicleStatisticsModel())));

            var result = (JsonResult)controller.OverdueList();
            var model = (JsonRemindersOverdueListViewModel)result.Data;
            var list = new List<OverdueReminderViewModel>(model.Reminders);

            Assert.Equal(0, list.Count());
        }

        [Fact]
        public void WhenOverdueListInvalidUser_ThenThrows()
        {
            ReminderController controller = GetTestableReminderController();

            controller.HttpContext.Request.SetAjaxRequest();
            controller.HttpContext.Request.SetJsonRequest();

            MockHandlerFor<GetOverdueRemindersForUser>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId))
                         .Throws(new UnauthorizedException("Unable to get overdue reminders.")));

            MockHandlerFor<GetVehicleById>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, defaultVehicleId))
                         .Returns(new VehicleModel(
                                      new Vehicle { VehicleId = defaultVehicleId, Name = "Vehicle" },
                                      new VehicleStatisticsModel())));

            Assert.Throws<UnauthorizedException>(() => { controller.OverdueList(); });
        }

        [Fact]
        public void WhenRetrievingJsonImminentReminders_ThenImminentReminders()
        {
            var controller = GetTestableReminderController();

            var dueDate = new DateTime(2010, 12, 1, 0, 0, 0, DateTimeKind.Utc);
            var reminders = new[]
            {
                new ImminentReminderModel(new Vehicle(),
                                                new Reminder
                                                    {
                                                        ReminderId = 1,
                                                        VehicleId = defaultVehicleId,
                                                        Title = "Reminder1",
                                                        DueDate = dueDate
                                                    }, isOverdue: true),
            };

            MockHandlerFor<GetImminentRemindersForUser>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, It.IsAny<DateTime>(), 0))
                         .Returns(reminders));

            MockHandlerFor<GetVehicleById>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, defaultVehicleId))
                         .Returns(new VehicleModel(
                                      new Vehicle { VehicleId = defaultVehicleId, Name = "Vehicle" },
                                      new VehicleStatisticsModel())));

            var result = (ContentTypeAwareResult)controller.ImminentReminders();
            Assert.NotNull(result);

            var actualModel = (IEnumerable<ImminentReminderModel>)result.Model;
            Assert.NotNull(actualModel);
            Assert.Equal(reminders.Count(), actualModel.Count());
        }

        [Fact]
        public void WhenGettingList_ThenRemindersReturned()
        {
            var vehicle = new VehicleModel(new Vehicle {VehicleId = defaultVehicleId},
                                           new VehicleStatisticsModel());

            var reminders = new[]
                                {
                                    new Reminder {ReminderId = 1, Title = "test reminder"},
                                    new Reminder {ReminderId = 2}
                                };

            MockHandlerFor<GetUnfulfilledRemindersForVehicle>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, defaultVehicleId, 0))
                         .Returns(reminders));

            MockHandlerFor<GetVehicleById>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, defaultVehicleId))
                         .Returns(vehicle));

            var controller = GetTestableReminderController();

            var result = (ContentTypeAwareResult)controller.ListPartial(defaultVehicleId);
            var data = (SelectedItemList<ReminderSummaryModel>)result.Model;

            Assert.NotNull(data);
            Assert.NotNull(data.List);
            Assert.Equal(reminders.Count(), data.List.Count());
            Assert.Equal("test reminder", data.List.First().Title);
        }

        [Fact]
        public void WhenCallingJsonFulfill_ThenDelegatesToBusinessServices()
        {
            const int reminderId = 321;

            var handler = MockHandlerFor<FulfillReminder>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.Id, reminderId))
                         .Verifiable("fulfill handler was not called"));

            var controller = GetTestableReminderController();

            controller.Fulfill(_defaultUser.Id, reminderId);

            handler.Verify();
        }

        [Fact]
        public void WhenGettingListByGroup_ThenRemindersReturned()
        {
            var vehicle = new VehicleModel(new Vehicle {VehicleId = defaultVehicleId},
                                           new VehicleStatisticsModel());

            var reminders = new[]
                                {
                                    new Reminder {Title = "active"},
                                    new Reminder {Title = "overdue", DueDate = DateTime.UtcNow.AddDays(-1)},
                                    new Reminder {Title = "fulfilled", IsFulfilled = true}
                                };

            MockHandlerFor<GetAllRemindersForVehicle>(
                x => x
                         .Setup(h => h.Execute(defaultVehicleId))
                         .Returns(reminders));

            MockHandlerFor<GetVehicleById>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, defaultVehicleId))
                         .Returns(vehicle));

            var controller = GetTestableReminderController();

            var result = (ContentTypeAwareResult)controller.ListByGroup(defaultVehicleId);
            var data = (List<ReminderListViewModel>)result.Model;

            Assert.NotNull(data);
            Assert.Equal(3, data.Count());
            Assert.Equal("active", data.First(d => d.Status == ReminderSummaryModel.StatusActive).Reminders.First().Title);
            Assert.Equal("overdue", data.First(d => d.Status == ReminderSummaryModel.StatusOverdue).Reminders.First().Title);
            Assert.Equal("fulfilled", data.First(d => d.Status == ReminderSummaryModel.StatusFulfilled).Reminders.First().Title);
        }

        private ReminderController GetTestableReminderController()
        {
            var controller = new ReminderController(_mockUserServices.Object, _serviceLocator.Object);
            controller.SetFakeControllerContext();
            controller.SetUserIdentity(new MileageStatsIdentity(_defaultUser.AuthorizationId,
                                                                _defaultUser.DisplayName,
                                                                _defaultUser.UserId));
            return controller;
        }

        private Mock<T> MockHandlerFor<T>(Action<Mock<T>> setup = null) where T : class
        {
            return _serviceLocator.MockHandlerFor(TestHelpExtensions.Mock<T>, setup);
        }

        private void MockDefaultHandlers()
        {
            MockHandlerFor<GetUnfulfilledRemindersForVehicle>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, defaultVehicleId, 0))
                         .Returns(new Reminder[] { }));

            MockHandlerFor<GetVehicleById>(
                x => x
                         .Setup(h => h.Execute(_defaultUser.UserId, defaultVehicleId))
                         .Returns(new VehicleModel(null, null)));

            MockHandlerFor<GetVehicleListForUser>(
                x => x.StandardSetup(_defaultUser.UserId, defaultVehicleId, defaultVehicleId));
        }
    }
}