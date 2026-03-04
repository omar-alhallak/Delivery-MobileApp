using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Drivers
{
    public class VehicleType
    {
        public VehicleTypeID VehicleID { get; private set; }
        public string VehicleName { get; private set; } 

        public double MaxDistanceKm { get; private set; }

        public double MaxMergeExtraKm { get; private set; }

        public int MaxOrdersToBatch { get; private set; }

        public int CommissionPercent { get; private set; }

        public bool IsActive { get; private set; }

        public VehicleType(VehicleTypeID id, string name, double maxDist, double maxExtra, int maxBatch, int commission)
        {
            VehicleID = id;
            VehicleName = name;
            MaxDistanceKm = maxDist;
            MaxMergeExtraKm = maxExtra;
            MaxOrdersToBatch = maxBatch;
            CommissionPercent = commission;
            IsActive = true;
        }

        public bool CanAcceptMoreOrders(int currentActiveOrders)
        {
            return currentActiveOrders < MaxOrdersToBatch;
        }
        public double GetTotalRangeLimit() => MaxDistanceKm + MaxMergeExtraKm;
    }
}