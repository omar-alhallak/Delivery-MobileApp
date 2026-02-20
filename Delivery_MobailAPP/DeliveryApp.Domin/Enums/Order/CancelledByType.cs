using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Enums.Order
{
    public enum CancelledByType : byte
    {
        None = 0,
        Customer = 1,
        Driver = 2,
        Merchant = 3,
        Admin = 4,
        System = 5
    }
}