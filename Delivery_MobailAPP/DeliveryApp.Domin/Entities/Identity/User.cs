using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.IdentityEnums;
using DeliveryApp.Domain.Enums.EngagementEnums;
using DeliveryApp.Domain.DomainErrors.IdentityErrors;

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

        public decimal CustomerAverageRating { get; private set; }
        public int CustomerRatingsCount { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? LastLoginAt { get; private set; }

        private User() { }

        public User(UserID id, UserRole roles, DateTimeOffset createdAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            ID = id;
            CreatedAt = createdAtUtc;

            RoleMask = UserRole.None;
            AddRoles(roles);

            IsProfileComplete = false;

            CustomerAverageRating = 0;
            CustomerRatingsCount = 0;
        }

        // ----------------------------
        //     Personal Information
        // ----------------------------

        public void AssignPublicID(PublicCode publicId)
        {
            if (PublicID is not null) throw new DomainConflictException
                    (UserErrors.PublicIdAlreadyAssignedCode, UserErrors.PublicIdAlreadyAssignedMessage);

            PublicID = publicId;
        }

        public void UpdateProfile(string? email, string? phone, string? fullName, string? photoUrl)
        {
            PreventModificationIfBanned();

            Email = Normalize(email)?.ToLowerInvariant();
            Phone = Normalize(phone);
            FullName = Normalize(fullName);
            PhotoUrl = Normalize(photoUrl);

            FieldLimits();

            if (IsProfileComplete && !ProfileCompletionRules()) throw new DomainRuleViolationException
                    (UserErrors.CantRemoveRequiredFieldCode, UserErrors.CantRemoveRequiredFieldMessage);
        }

        public void ProfileComplete()
        {
            PreventModificationIfBanned();

            if (!ProfileCompletionRules()) throw new DomainRuleViolationException
                    (UserErrors.ProfileFieldNotCompleteCode, UserErrors.ProfileFieldNotCompleteMessage);

            IsProfileComplete = true;
        }

        public void ProfileNotComplete()
        {
            PreventModificationIfBanned();
            IsProfileComplete = false;
        }

        private bool ProfileCompletionRules() => Phone is not null && FullName is not null;

        private void FieldLimits()
        {
            if (Email is not null && Email.Length > 255) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Email));

            if (Phone is not null && Phone.Length > 16) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Phone));

            if (FullName is not null && FullName.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(FullName));

            if (PhotoUrl is not null && PhotoUrl.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(PhotoUrl));
        }

        // -------------------------
        //        Roles
        // -------------------------

        public bool HasRole(UserRole role) => (RoleMask & role) == role;

        public void AddRoles(UserRole roles)
        {
            PreventModificationIfBanned();

            foreach (var role in SplitRoleMask(roles))
            {
                AddSingleRole(role);
            }
        }

        public void RemoveRole(UserRole role)
        {
            PreventModificationIfBanned();

            if (role == UserRole.None) return;

            if ((role & UserRole.Customer) == UserRole.Customer && HasRole(UserRole.Driver)) throw new DomainRuleViolationException
                    (UserErrors.CantRemoveCustFromDrivCode, UserErrors.CantRemoveCustFromDrivMessage);

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

        public void Suspend(DateTimeOffset? untilUtc, DateTimeOffset utcNow)
        {
            PreventModificationIfBanned();

            if (untilUtc.HasValue && untilUtc <= utcNow) throw new DomainValidationException
                    (UserErrors.SuspensionMustBeFutureCode, UserErrors.SuspensionMustBeFutureMessage, nameof(untilUtc));

            AccountStatus = AccountStatus.Suspended;
            SuspendedUntilUtc = untilUtc;
        }

        public void Ban()
        {
            AccountStatus = AccountStatus.Banned;
            SuspendedUntilUtc = null;
        }

        public bool IsSuspensionExpired(DateTimeOffset utcNow) =>
            AccountStatus == AccountStatus.Suspended &&
            SuspendedUntilUtc.HasValue &&
            utcNow >= SuspendedUntilUtc.Value;

        public void AutoActivateIfExpired(DateTimeOffset utcNow)
        {
            if (IsSuspensionExpired(utcNow))
                Activate();
        }

        public void SetLastLogin(DateTimeOffset utcNow)
        {
            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            LastLoginAt = utcNow;
        }

        private void PreventModificationIfBanned()
        {
            if (AccountStatus == AccountStatus.Banned) throw new DomainRuleViolationException
                    (UserErrors.BannedCantBeModifiedCode, UserErrors.BannedCantBeModifiedMessage);
        }

        private static string? Normalize(string? s) =>
            string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        // -------------------------
        //        Rating
        // -------------------------

        public void AddCustomerRating(RatingStars stars)
        {
            ValidateRatingStars(stars);

            var value = (int)stars;

            CustomerAverageRating = ((CustomerAverageRating * CustomerRatingsCount) + value) / (CustomerRatingsCount + 1);
            CustomerRatingsCount++;
        }

        public void UpdateCustomerRating(RatingStars oldStars, RatingStars newStars)
        {
            ValidateRatingStars(oldStars);
            ValidateRatingStars(newStars);

            if (CustomerRatingsCount <= 0) throw new DomainRuleViolationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage);

            var oldValue = (int)oldStars;
            var newValue = (int)newStars;

            CustomerAverageRating = ((CustomerAverageRating * CustomerRatingsCount) - oldValue + newValue) / CustomerRatingsCount;
        }

        private static void ValidateRatingStars(RatingStars stars)
        {
            if (!Enum.IsDefined(typeof(RatingStars), stars)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(stars));
        }
    }
}