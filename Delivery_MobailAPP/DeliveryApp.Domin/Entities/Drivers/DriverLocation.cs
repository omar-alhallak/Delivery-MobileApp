using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Drivers
{
    public class DriverLocation
    {
        // استخدام الـ Aliases الجديدة مباشرة
        public DriverLocationID ID { get; private set; }
        public DriverID DriverId { get; private set; }

        public decimal Latitude { get; private set; }
        public decimal Longitude { get; private set; }
        public DateTimeOffset RecordedAt { get; private set; }

        private DriverLocation() { }

        public DriverLocation(
            DriverID driverId,
            decimal lat,
            decimal lng,
            DateTimeOffset recordedAtUtc)
        {
            if (driverId.IsEmpty)
                throw new ArgumentException("DriverId is required.");

            if (lat < -90 || lat > 90) throw new ArgumentOutOfRangeException(nameof(lat));
            if (lng < -180 || lng > 180) throw new ArgumentOutOfRangeException(nameof(lng));

            ID = DriverLocationID.New(); // توليد معرف جديد للسجل
            DriverId = driverId;
            Latitude = lat;
            Longitude = lng;
            RecordedAt = recordedAtUtc;
        }
    }
}