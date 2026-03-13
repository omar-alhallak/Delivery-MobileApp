using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Enums.AccountWarningEnums
{
    public enum WarningReason:byte
    {
        AbusiveLanguage = 1,
        LateDelivery = 2,
        FakeOrder = 3,
        Fraud = 4,
        PolicyViolation = 5,
        Other = 10
    }
}
