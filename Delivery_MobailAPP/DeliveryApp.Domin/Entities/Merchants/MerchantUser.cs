using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Enums;
using DeliveryApp.Domain.ValueObjects;

namespace DeliveryApp.Domain.Entities.Merchants
{
    public class MerchantUser
    {
        public MerchantID ID { get; private set; }

        public PublicCode? PublicID { get; private set; }

        public Merchant? Merchant { get; private set; }
        public Guid UserID { get; private set; }

        public User? user { get; private set; }

        public MerchantUserRole Role { get; private set; }

        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private MerchantUser() { }

        public MerchantUser(MerchantID iD, Guid userID, MerchantUserRole role)
        {
            ID = iD;
            UserID = userID;
            Role = role;
            IsActive = true;
            CreatedAt = DateTimeOffset.UtcNow;
            Role = MerchantUserRole.None;
            UpdateMerchantUser(role, iD, userID);
        }
        public void UpdateMerchantUser(MerchantUserRole role, MerchantID iD, Guid userID)
        {
            ID = iD;
            UserID = userID;
            Role = role;
            IsActive = true;
            CreatedAt = DateTimeOffset.UtcNow;
            FieldLimits();
        }

        private void FieldLimits()
        {
            if (ID.Value == Guid.Empty)
                throw new ArgumentException("A valid merchant ID must be provided; it cannot be blank.");

            if (UserID == Guid.Empty)
                throw new ArgumentException("A valid user ID must be provided to link it to the merchant.");

            if (Role >= 0)
                throw new ArgumentException("A valid user role number must be specified.");

            if (Role == Role && ID == ID && UserID == UserID)
                throw new ArgumentException("No changes detected in the merchant user details; update operation is redundant.");
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
                throw new InvalidOperationException("The owner cannot be removed without at least one replacement employee.");

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