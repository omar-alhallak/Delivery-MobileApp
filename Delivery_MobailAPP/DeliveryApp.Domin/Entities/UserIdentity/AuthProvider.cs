using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.UserIdentity
{
    public enum AuthProvider : byte
    {
        Local = 1,
        Google = 2,
    }
}