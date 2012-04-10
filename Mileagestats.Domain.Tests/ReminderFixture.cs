/*  
Copyright Microsoft Corporation

Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at 

http://www.apache.org/licenses/LICENSE-2.0 

THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
MERCHANTABLITY OR NON-INFRINGEMENT. 

See the Apache 2 License for the specific language governing permissions and
limitations under the License. */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MileageStats.Web.Models;
using Xunit;

namespace MileageStats.Domain.Tests
{
    public class ReminderFixture
    {
        [Fact]
        public void WhenNeitherReminderDueDateNorDueDistanceSet_ThenValidationFails()
        {
            var target = new ReminderFormModel {Title = "Title"};

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
        }

        [Fact]
        public void WhenConstructed_ThenSuccessful()
        {
            var actual = new ReminderFormModel();

            Assert.NotNull(actual);
            Assert.Equal(0, actual.ReminderId);
            Assert.Null(actual.Title);
            Assert.Null(actual.Remarks);
            Assert.Null(actual.DueDate);
            Assert.Null(actual.DueDistance);
        }

        [Fact]
        public void WhenConstructed_ThenIsFulfilledIsFalse()
        {
            var target = new ReminderFormModel();

            Assert.False(target.IsFulfilled);
        }

        [Fact]
        public void WhenReminderIdSet_ThenValueUpdated()
        {
            var target = new ReminderFormModel();

            target.ReminderId = 4;

            int actual = target.ReminderId;
            Assert.Equal(4, actual);
        }

        [Fact]
        public void WhenTitleSet_ThenValueUpdated()
        {
            var target = new ReminderFormModel();

            target.Title = "Title";

            string actual = target.Title;
            Assert.Equal("Title", actual);
        }

        [Fact]
        public void WhenTitleSetToNull_ThenUpdatesValue()
        {
            var target = new ReminderFormModel();
            target.Title = "Title";

            target.Title = null;

            string actual = target.Title;
            Assert.Null(actual);
        }

        [Fact]
        public void WhenTitleSetToValidValue_ThenValidationPasses()
        {
            var target = new ReminderFormModel();

            target.Title = "Title";
            target.DueDate = DateTime.UtcNow;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenTitleSetToNull_ThenValidationFails()
        {
            var target = new ReminderFormModel();

            target.Title = null;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults.Count);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("Title", validationResults[0].MemberNames.First());
        }

        [Fact]
        public void WhenTitleSetToEmpty_ThenValidationFails()
        {
            var target = new ReminderFormModel();

            target.Title = string.Empty;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("Title", validationResults[0].MemberNames.First());
        }

        [Fact]
        public void WhenTitleSetTo51Characters_ThenValidationFails()
        {
            var target = new ReminderFormModel();

            target.Title = new string('1', 51);

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("Title", validationResults[0].MemberNames.First());
        }

        [Fact]
        public void WhenRemarksSet_ThenValueUpdated()
        {
            var target = new ReminderFormModel();

            target.Remarks = "Remarks";

            string actual = target.Remarks;
            Assert.Equal("Remarks", actual);
        }

        [Fact]
        public void WhenRemarksSetToNull_ThenUpdatesValue()
        {
            var target = new ReminderFormModel();
            target.Remarks = "Remarks";

            target.Remarks = null;

            string actual = target.Remarks;
            Assert.Null(actual);
        }

        [Fact]
        public void WhenRemarksSetToValidValue_ThenValidationPasses()
        {
            var target = new ReminderFormModel();
            target.Title = "Title";
            target.DueDate = DateTime.UtcNow;

            target.Remarks = "Remarks";

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenRemarksSetToNull_ThenValidationPasses()
        {
            var target = new ReminderFormModel();
            target.Title = "Title";
            target.DueDate = DateTime.UtcNow;

            target.Remarks = null;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenRemarksSetToEmpty_ThenValidationPasses()
        {
            var target = new ReminderFormModel();
            target.Title = "Title";
            target.DueDate = DateTime.UtcNow;

            target.Remarks = string.Empty;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenRemarksSetTo251Characters_ThenValidationFails()
        {
            var target = new ReminderFormModel();
            target.Title = "Title";

            target.Remarks = new string('1', 251);

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("Remarks", validationResults[0].MemberNames.First());
        }

        [Fact]
        public void WhenDueDateSet_ThenValueUpdated()
        {
            var target = new ReminderFormModel();

            target.DueDate = new DateTime(2011, 01, 01);

            DateTime? actual = target.DueDate;
            Assert.NotNull(actual);
            Assert.Equal(new DateTime(2011, 01, 01), actual);
        }

        [Fact]
        public void WhenDueDateSetToNull_ThenUpdatesValue()
        {
            var target = new ReminderFormModel();
            target.DueDate = new DateTime(2011, 01, 01);

            target.DueDate = null;

            DateTime? actual = target.DueDate;
            Assert.Null(actual);
        }

        [Fact]
        public void WhenDueDistanceSet_ThenValueUpdated()
        {
            var target = new ReminderFormModel();

            target.DueDistance = 5;

            int? actual = target.DueDistance;
            Assert.NotNull(actual);
            Assert.Equal(5, actual);
        }

        [Fact]
        public void WhenDueDistanceSetToNull_ThenUpdatesValue()
        {
            var target = new ReminderFormModel();
            target.DueDistance = 5;

            target.DueDistance = null;

            int? actual = target.DueDistance;
            Assert.Null(actual);
        }

        [Fact]
        public void WhenDueDistanceSetToValidValue_ThenValidationPasses()
        {
            var target = new ReminderFormModel();
            target.Title = "Title";

            target.DueDistance = 5;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.True(actual);
            Assert.Equal(0, validationResults.Count);
        }

        [Fact]
        public void WhenDueDistanceSetToNegative_ThenValidationFails()
        {
            var target = new ReminderFormModel();
            target.Title = "Title";

            target.DueDistance = -5;

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
            Assert.Equal(1, validationResults[0].MemberNames.Count());
            Assert.Equal("DueDistance", validationResults[0].MemberNames.First());
        }

        [Fact]
        public void WhenReminderIsNotOverdue_ThenIsOverdueReturnsFalse()
        {
            var reminder = new ReminderFormModel()
                               {Title = "future reminder", DueDate = DateTime.UtcNow.AddDays(2), DueDistance = 10000};

            reminder.UpdateLastVehicleOdometer(10);
            bool isOverdue = reminder.IsOverdue;

            Assert.False(isOverdue);
        }

        [Fact]
        public void WhenReminderIsOverdueByDate_ThenIdOverdueReturnsTrue()
        {
            var reminder = new ReminderFormModel()
                               {Title = "future reminder", DueDate = DateTime.UtcNow.AddDays(-2), DueDistance = 10000};

            reminder.UpdateLastVehicleOdometer(10);
            bool isOverdue = reminder.IsOverdue;

            Assert.True(isOverdue);
        }

        [Fact]
        public void WhenReminderIsOverdueByDistance_ThenIdOverdueReturnsTrue()
        {
            var reminder = new ReminderFormModel()
                               {Title = "future reminder", DueDate = DateTime.UtcNow.AddDays(2), DueDistance = 10000};

            reminder.UpdateLastVehicleOdometer(12000);
            bool isOverdue = reminder.IsOverdue;

            Assert.True(isOverdue);
        }

        [Fact]
        public void WhenReminderIsOverdueByDateAndDistance_ThenIdOverdueReturnsTrue()
        {
            var reminder = new ReminderFormModel()
                               {Title = "future reminder", DueDate = DateTime.UtcNow.AddDays(-2), DueDistance = 10000};

            reminder.UpdateLastVehicleOdometer(12000);
            bool isOverdue = reminder.IsOverdue;

            Assert.True(isOverdue);
        }

        [Fact]
        public void WhenReminderIsOverdueByDistanceAndDueDateNull_ThenIsOverdueReturnsTrue()
        {
            var reminder = new ReminderFormModel() {Title = "future reminder", DueDate = null, DueDistance = 10000};

            reminder.UpdateLastVehicleOdometer(12000);
            bool isOverdue = reminder.IsOverdue;

            Assert.True(isOverdue);
        }

        [Fact]
        public void WhenReminderIsOverdueByDateAndDueDistanceNull_ThenIsOverdueReturnsTrue()
        {
            var reminder = new ReminderFormModel()
                               {Title = "future reminder", DueDate = DateTime.UtcNow.AddDays(-2), DueDistance = null};

            reminder.UpdateLastVehicleOdometer(10);
            bool isOverdue = reminder.IsOverdue;

            Assert.True(isOverdue);
        }

        [Fact]
        public void WhenReminderDueDateAndDueDistanceNull_ThenIsOverdueReturnsFalse()
        {
            var reminder = new ReminderFormModel() {Title = "future reminder", DueDate = null, DueDistance = null};

            reminder.UpdateLastVehicleOdometer(10);
            bool isOverdue = reminder.IsOverdue;

            Assert.False(isOverdue);
        }


        [Fact]
        public void WhenNeitherDueDateNorDueDistanceSet_ThenValidationFails()
        {
            var target = new ReminderFormModel();
            target.Title = "Title";

            var validationContext = new ValidationContext(target, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(target, validationContext, validationResults, true);

            Assert.False(actual);
        }
    }
}