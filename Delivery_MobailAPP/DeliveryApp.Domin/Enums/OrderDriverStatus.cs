using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Enums
{
    public enum OrderDriverStatus
    {
        Assigned = 1,
        Accepted = 2,
        Rejected = 3,
        Completed = 4,
        Removed = 5
    }
}