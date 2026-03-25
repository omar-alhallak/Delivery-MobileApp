using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Domain.Entities.Merchants
{
    public class MerchantUser
    {
        public MerchantID MerchantID { get; private set; }
        public UserID UserID { get; private set; }

        public MerchantUserRole Role { get; private set; }

        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private MerchantUser() { }

        public MerchantUser(MerchantID MerchantId, UserID UserId, MerchantUserRole role, DateTimeOffset CreatedAtUtc)
        {
            if (MerchantId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(MerchantId));

            if (UserId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(UserId));

            if (CreatedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(CreatedAtUtc));

            if (!Enum.IsDefined(typeof(MerchantUserRole), role)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(role));

            MerchantID = MerchantId;
            UserID = UserId;
            Role = role;
            CreatedAt = CreatedAtUtc;
            IsActive = true;
        }

        public void ChangeRole(MerchantUserRole role)
        {
            if (!Enum.IsDefined(typeof(MerchantUserRole), role)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(role));

            Role = role;
        }

        public void Activate()
        {
            if (IsActive) return;

            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive) return;

            IsActive = false;
        }
    }
}