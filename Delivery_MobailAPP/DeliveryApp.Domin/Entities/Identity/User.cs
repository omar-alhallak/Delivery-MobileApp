using DeliveryApp.Domain.Enums;
using DeliveryApp.Domain.ValueObjects;

namespace DeliveryApp.Domain.Entities.Identity
{
    public class User
    {
        public UserID ID { get; private set; }
        public PublicCode? PublicID { get; private set; }

        public string? Email { get; private set; }
        public string? FullName { get; private set; }
        public string? Phone { get; private set; }
        public string? PhotoUrl { get; private set; }

        public bool IsProfileComplete { get; private set; }

        public UserRole RoleMask { get; private set; }

        public AccountStatus AccountStatus { get; private set; } = AccountStatus.Active;
        public DateTimeOffset? SuspendedUntilUtc { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? LastLoginAt { get; private set; }

        private User() { } 

        public User(UserID id, UserRole Roles, DateTimeOffset CreatedAtUtc)
        {
            ID = id;
            CreatedAt = CreatedAtUtc;

            RoleMask = UserRole.None;
            AddRoles(Roles);

            IsProfileComplete = false;
        }

        // ----------------------------
        //     Personal Information
        // ----------------------------

        public void AssignPublicID(PublicCode publicId) 
        {
            if (PublicID is not null)
                throw new InvalidOperationException("PublicID already assigned.");

            PublicID = publicId;
        }

        public void UpdateProfile(string? email, string? phone, string? fullName, string? photoUrl)
        {
            PreventModificationIfNotBanned();

            Email = Normalize(email)?.ToLowerInvariant();
            Phone = Normalize(phone);
            FullName = Normalize(fullName);
            PhotoUrl = Normalize(photoUrl);

            FieldLimits();

            if (IsProfileComplete && !ProfileCompletionRules())
                throw new InvalidOperationException("Profile is complete, required fields cannot be cleared.");
        }

        public void ProfileComplete()
        {
            PreventModificationIfNotBanned();

            if (!ProfileCompletionRules())
                throw new InvalidOperationException("Phone and FullName are required.");

            IsProfileComplete = true;
        }

        public void ProfileNotcomplete()
        {
            PreventModificationIfNotBanned();
            IsProfileComplete = false;
        }

        private bool ProfileCompletionRules() => Phone is not null && FullName is not null;

        private void FieldLimits()
        {
            if (Email is not null && Email.Length > 255)
                throw new ArgumentException("Email too long.");

            if (Phone is not null && Phone.Length > 16)
                throw new ArgumentException("Phone too long.");

            if (FullName is not null && FullName.Length > 150)
                throw new ArgumentException("FullName too long.");

            if (PhotoUrl is not null && PhotoUrl.Length > 500)
                throw new ArgumentException("PhotoUrl too long.");
        }

        // -------------------------
        //        Roles
        // -------------------------

        public bool HasRole(UserRole role) => (RoleMask & role) == role;

        public void AddRoles(UserRole roles)
        {
            PreventModificationIfNotBanned();

            foreach (var role in SplitRoleMask(roles))
                AddSingleRole(role);
        }

        public void RemoveRole(UserRole role)
        {
            PreventModificationIfNotBanned();

            if (role == UserRole.None) return;

            if ((role & UserRole.Customer) == UserRole.Customer && HasRole(UserRole.Driver))
                throw new InvalidOperationException("Cannot remove Customer role from a Driver.");

            RoleMask &= ~role;

            CheckDriverIsCustomer();
        }

        private void AddSingleRole(UserRole role)
        {
            if (role == UserRole.None) return;

            if (role == UserRole.Driver)
                RoleMask |= UserRole.Customer;

            RoleMask |= role;

            CheckDriverIsCustomer();
        }

        private void CheckDriverIsCustomer()
        {
            if (HasRole(UserRole.Driver) && !HasRole(UserRole.Customer))
                RoleMask |= UserRole.Customer;
        }

        private static IEnumerable<UserRole> SplitRoleMask(UserRole roles)
        {
            foreach (UserRole r in Enum.GetValues(typeof(UserRole)))
            {
                if (r == UserRole.None) continue;
                if ((roles & r) == r) yield return r;
            }
        }

        // -------------------------
        //        Status
        // -------------------------

        public void Activate()
        {
            AccountStatus = AccountStatus.Active;
            SuspendedUntilUtc = null;
        }

        public void Suspend(DateTimeOffset? UntilUtc)
        {
            PreventModificationIfNotBanned();

            if (UntilUtc.HasValue && UntilUtc <= DateTimeOffset.UtcNow)
                throw new InvalidOperationException("Suspension must be in the future.");

            AccountStatus = AccountStatus.Suspended;
            SuspendedUntilUtc = UntilUtc;
        }

        public void Ban()
        {
            AccountStatus = AccountStatus.Banned;
            SuspendedUntilUtc = null;
        }

        public bool IsSuspensionExpired(DateTimeOffset UtcNow) => AccountStatus == AccountStatus.Suspended
               && SuspendedUntilUtc.HasValue && UtcNow >= SuspendedUntilUtc.Value;

        public void AutoActivateIfExpired(DateTimeOffset UtcNow)
        {
            if (IsSuspensionExpired(UtcNow))
                Activate();
        }

        public void SetLastLogin(DateTimeOffset UtcNow) => LastLoginAt = UtcNow;

        private void PreventModificationIfNotBanned()
        {
            if (AccountStatus == AccountStatus.Banned)
                throw new InvalidOperationException("Banned user cannot be modified.");
        }

        private static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }
}