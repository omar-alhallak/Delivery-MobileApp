using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Enums.AccountWarningEnums
{
    public enum WarningDecision:byte
    {
        Pending = 1,
        Confirmed = 2,
        Dismissed = 3
    }
}
