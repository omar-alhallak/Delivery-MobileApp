using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities
{
    internal class Zone
    {

        public Guid ZoneId { get; private set; }



        public string? ZoneName { get; private set; }



        public bool IsActive { get; private set; }


        public bool IsServiceable { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private Zone() { }

        public Zone(string name, decimal lat, decimal lng, decimal fee = 0)
        {
            ZoneId = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;

            UpdateDetails(name, lat, lng, fee);
        }

        public void UpdateDetails(string name, decimal lat, decimal lng, decimal fee)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Zone name is required");

            ZoneName = name.Trim();

        }
        public void SetStatus(bool isActive) => IsActive = isActive;

    }
}

