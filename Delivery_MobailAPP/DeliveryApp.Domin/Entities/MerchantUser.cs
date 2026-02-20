using DeliveryApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities
{
    public class MerchantUser
    {
        public Guid MerchantID { get; private set; }
        public Guid UserID { get; private set; }

        public MerchantUserRole Role { get; private set; }

        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private MerchantUser() { }

        public MerchantUser(Guid MerchantId, Guid UserId, MerchantUserRole role)
        {
            if (MerchantId == Guid.Empty)
                throw new ArgumentException("MerchantId cannot be empty.");

            if (UserId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            MerchantID = MerchantId;
            UserID = UserId;
            Role = role;
            IsActive = true;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public void DeActivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void ChangeRole(MerchantUserRole newRole)
        {
            Role = newRole;
        }
    }
}