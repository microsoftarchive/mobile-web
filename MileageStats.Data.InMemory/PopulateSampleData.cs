/*  
Copyright Microsoft Corporation

Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at 

http://www.apache.org/licenses/LICENSE-2.0 

THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
MERCHANTABLITY OR NON-INFRINGEMENT. 

See the Apache 2 License for the specific language governing permissions and
limitations under the License. */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using MileageStats.Domain.Contracts.Data;
using MileageStats.Domain.Models;

namespace MileageStats.Data.InMemory
{
    public class PopulateSampleData
    {
        private readonly int[] _distance = new[] {350, 310, 360, 220, 310, 360, 350, 340, 375, 410, 270, 330};
        private readonly Double[] _fee = new[] {.45, 0, .50, 0, 0, 0, .30, .45, .50, 0, .45, 0};
        private readonly Double[] _price = new[] {3.5, 3.75, 3.75, 3.65, 3.45, 3.75, 3.75, 3.70, 3.5, 3.65, 3.70, 3.35};
        private readonly Double[] _units = new[] {17, 14, 16, 12, 17, 18, 16.5, 17, 17, 19, 14, 17};

        private readonly IUserRepository _users;
        private readonly IReminderRepository _reminders;
        private readonly IFillupRepository _fillups;
        private readonly IVehicleRepository _vehicles;
        private readonly IVehiclePhotoRepository _photos;

        private readonly String[] _vendor = new[]
                                                {
                                                    "Fabrikam", "Contoso", "Margie's Travel", "Adventure Works",
                                                    "Fabrikam"
                                                    , "Contoso", "Margie's Travel", "Adventure Works", "Fabrikam",
                                                    "Contoso", "Margie's Travel", "Adventure Works"
                                                };

        public PopulateSampleData(IVehicleRepository vehicles, IVehiclePhotoRepository photos, IUserRepository users, IReminderRepository reminders, IFillupRepository fillups)
        {
            _vehicles = vehicles;
            _photos = photos;
            _users = users;
            _reminders = reminders;
            _fillups = fillups;
        }

        public void Seed(int? userId)
        {
            if(!userId.HasValue)
            {
                var user = new User
                {
                    AuthorizationId = "http://not/a/real/openid/url",
                    DisplayName = "Sample User",
                    Country = "United States"
                };
                _users.Create(user);
                userId = user.UserId;
            }

            SeedVehicles(userId.Value);
        }

        private void SeedVehicles(int userId)
        {
            var cars = new[]
                           {
                               new Vehicle
                                   {
                                       UserId = userId,
                                       Name = "Hot Rod",
                                       SortOrder = 1,
                                       Year = 2003,
                                       MakeName = "BMW",
                                       ModelName = "330xi"
                                   },
                               new Vehicle
                                   {
                                       UserId = userId,
                                       Name = "Soccer Mom's Ride",
                                       SortOrder = 2,
                                       Year = 1997,
                                       MakeName = "Honda",
                                       ModelName = "Accord LX"
                                   },
                               new Vehicle
                                   {
                                       UserId = userId,
                                       Name = "Mud Lover",
                                       SortOrder = 3,
                                       Year = 2011,
                                       MakeName = "Jeep",
                                       ModelName = "Wrangler"
                                   }
                           };

            var now = DateTime.Now;
            var fillupdata = new[]
                                 {
                                     new FillupData {Mileage = 1000, Units = 1, Distance = 1, Date = now.AddDays(-365)},
                                     new FillupData
                                         {Mileage = 500, Units = 0.9, Distance = 1.2, Date = now.AddDays(-370)},
                                     new FillupData
                                         {Mileage = 750, Units = 1.2, Distance = 0.8, Date = now.AddDays(-373)},
                                 };
            cars
                .Select((car, index) =>
                            {
                                CreateVehicle(userId, car);
                                CreateFillups(car, fillupdata[index]);
                                CreateReminders(car);

                                return car;
                            })
                .ToList(); // ToList to force execution
        }

        private VehiclePhoto CreateVehiclePhoto(Image image, int vehicleId)
        {
            byte[] buffer;
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, new ImageFormat(image.RawFormat.Guid));
                buffer = memoryStream.ToArray();
            }
            var vehiclePhoto = new VehiclePhoto {ImageMimeType = "image/jpeg", Image = buffer, VehicleId = vehicleId};
            _photos.Create(vehicleId, vehiclePhoto);
            return vehiclePhoto;
        }

        private void CreateVehicle(int userId, Vehicle vehicle)
        {
            _vehicles.Create(userId, vehicle);

            string photoId = string.Format("MileageStats.Data.InMemory.Photos.{0}.jpg", vehicle.MakeName);
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(photoId);
            vehicle.Photo = CreateVehiclePhoto(Image.FromStream(stream), vehicle.VehicleId);
            vehicle.PhotoId = vehicle.Photo.VehiclePhotoId;
        }

        private void CreateReminders(Vehicle vehicle)
        {
            var lastFillup = _fillups.GetFillups(vehicle.VehicleId).OrderByDescending(f => f.Date).FirstOrDefault();
            if (lastFillup == null)
            {
                return;
            }

            // create "overdue by mileage" reminder
            vehicle.Reminders.Add(new Reminder
                           {
                               DueDate = null,
                               DueDistance = lastFillup.Odometer - 10,
                               IsFulfilled = false,
                               Remarks = "Check air filter when oil is changed",
                               Title = "Oil Change",
                               VehicleId = vehicle.VehicleId
                           });

            // create "overdue by date" reminder
            vehicle.Reminders.Add(new Reminder
                           {
                               DueDate = lastFillup.Date.AddDays(-10),
                               DueDistance = null,
                               IsFulfilled = false,
                               Remarks = "Check condition of the wipers",
                               Title = "Check Wiper Fluid",
                               VehicleId = vehicle.VehicleId
                           });

            // create "to be done soon by mileage" reminder
            vehicle.Reminders.Add(new Reminder
                           {
                               DueDate = null,
                               DueDistance = lastFillup.Odometer + 400,
                               IsFulfilled = false,
                               Remarks = "Check air pressure",
                               Title = "Rotate Tires",
                               VehicleId = vehicle.VehicleId
                           });

            // create "to be done soon by date" reminder
            vehicle.Reminders.Add(new Reminder
                           {
                               DueDate = DateTime.Now.AddDays(+10),
                               DueDistance = null,
                               IsFulfilled = false,
                               Remarks = "Check air freshener",
                               Title = "Vacuum Car",
                               VehicleId = vehicle.VehicleId
                           });

            vehicle.Reminders.ToList()
                .ForEach(reminder => _reminders.Create(vehicle.VehicleId,reminder));
        }

        private void CreateFillups(Vehicle vehicle, FillupData data)
        {
            int odometer = data.Mileage;
            DateTime date = data.Date;
            Double unitsModifier = data.Units;
            Double distanceModifier = data.Distance;
            int[] randomArray = RandomizeArray(new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11});
            int currentIndex = 0;
            var random = new Random();
            bool isFirst = true;
            while (date < DateTime.Now)
            {
                int dataIndex = randomArray[currentIndex];
                var distance = (int) (_distance[dataIndex]*distanceModifier);
                var fillup = new FillupEntry
                                 {
                                     Date = date,
                                     Odometer = odometer,
                                     PricePerUnit = _price[dataIndex],
                                     TotalUnits = _units[dataIndex]*unitsModifier,
                                     TransactionFee = _fee[dataIndex],
                                     VehicleId = vehicle.VehicleId,
                                     Vendor = _vendor[dataIndex]
                                 };
                if (isFirst)
                {
                    isFirst = false;
                    fillup.Distance = null;
                }
                else
                {
                    fillup.Distance = distance;
                }
                odometer += distance;

                currentIndex += 1;
                if (currentIndex > 11)
                {
                    currentIndex = 0;
                }
                date = date.AddDays(random.Next(3, 14));
                _fillups.Create(vehicle.UserId, vehicle.VehicleId, fillup);
            }
        }

        private int[] RandomizeArray(int[] array)
        {
            var random = new Random();
            for (int i = array.Length - 1; i > 0; i--)
            {
                int swapPosition = random.Next(i + 1);
                int temp = array[i];
                array[i] = array[swapPosition];
                array[swapPosition] = temp;
            }
            return array;
        }

        #region Nested type: FillupData

        private struct FillupData
        {
            public DateTime Date;
            public double Distance;
            public int Mileage;
            public double Units;
        }

        #endregion
    }
}