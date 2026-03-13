using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Enums.ComplaintEnums
{
    public enum ComplaintReason : byte
    {
        LateDelivery = 1,
        AbusiveBehavior = 2,
        WrongOrder = 3,
        OrderNotReceived = 4,
        PaymentIssue = 5,
        Other = 10
    }
}
