using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Order
{
    public enum PaymentStatus : byte
    {
        UnPaid = 0,
        Paid = 1,
    }
}
