using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Enums
{
    public enum MerchantUserRole : byte
    {
        None = 0,
        Owner = 1,
        Staff = 2,
    }
}