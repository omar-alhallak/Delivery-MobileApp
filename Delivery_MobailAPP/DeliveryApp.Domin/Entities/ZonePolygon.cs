using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities
{
    internal class ZonePolygon
    {

        public Guid PointId { get; private set; }

        public Guid ZoneId { get; private set; }
        public Zone ?Zone { get; private set; }= null!;


        public decimal Latitude { get; private set; }


        public decimal Longitude { get; private set; }

        public enum Staut
        {
            Active = 1,
            Inactive = 2,
            OutOfStock = 3,
            Archived = 4
        }
        public int SortOrder { get; private set; }

        public DateTimeOffset CreateAt { get; private set; }

        private ZonePolygon() { }

        public ZonePolygon(Guid zoneId, decimal lat, decimal lng, int sortOrder)
        {
            PointId = Guid.NewGuid();
            ZoneId = zoneId;
            Latitude = lat;
            Longitude = lng;
            SortOrder = sortOrder;
        }
        public void UpdateLocation(decimal lat, decimal lng)
        {
            if (lat < -90 || lat > 90)
                throw new ArgumentOutOfRangeException(nameof(lat), "The latitude should be between - 90 and 90.");

            if (lng < -180 || lng > 180)
                throw new ArgumentOutOfRangeException(nameof(lng), "The longitude line should be between -180 and 180");

            this.Latitude = lat;
            this.Longitude = lng;
        }
    }
}
