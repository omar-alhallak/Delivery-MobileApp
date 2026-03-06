using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Enums;

namespace DeliveryApp.Domain.Entities.Merchants
{
    public class MerchantUser
    {
        public  MerchantID MerchantID { get; private set; }
        public  UserID UserID { get; private set; }
        public Merchant? Merchant { get; private set; }

        public User? User { get; private set; }

        public MerchantUserRole Role { get; private set; }

        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private MerchantUser() { }

        public MerchantUser(MerchantID merchantID, UserID userID, MerchantUserRole role)
        {
            MerchantID = merchantID;
            UserID = userID;
            CreatedAt = DateTimeOffset.UtcNow;    
        }
        public void UpdateMerchantUser(MerchantID iD, UserID id, MerchantUserRole role)
        {
            Role = role;
            IsActive = true;
            FieldLimits();
        }
        private void FieldLimits()
        {
            if (MerchantID.IsEmpty)
                throw new DomainRuleViolationException(ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage);
            if (UserID.IsEmpty)
                throw new DomainRuleViolationException(ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage);
        }
        public bool HasRole(MerchantUserRole role) => (Role & role) == role;
        public void AddRoles(MerchantUserRole roles)
        {
            foreach (var r in SplitRoleMask(roles))
                AddSingleRole(r);
        }
        public void RemoveRole(MerchantUserRole role)
        {
            if (role == MerchantUserRole.None) return;
            if (role == MerchantUserRole.Owner && !HasRole(MerchantUserRole.Staff))

                throw new DomainRuleViolationException(MerchantErrors.OwnerRequiredCode,MerchantErrors.OwnerRequiredMessage);
            Role &= ~role;
        }
        private void AddSingleRole(MerchantUserRole role)
        {
            if (role == MerchantUserRole.None) return;
            Role |= role;
        }
        private static IEnumerable<MerchantUserRole> SplitRoleMask(MerchantUserRole roles)
        {
            foreach (MerchantUserRole r in Enum.GetValues(typeof(MerchantUserRole)))
            {
                if (r == MerchantUserRole.None) continue;
                if ((roles & r) == r) yield return r;
            }
        }
        private static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }
}