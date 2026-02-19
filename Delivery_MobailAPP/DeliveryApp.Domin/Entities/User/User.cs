using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.User
{
    public class User
    {
        public Guid UserID { get; private set; }
        public string? PublicUserID { get; private set; }

        public string? Email { get; private set; }
        public string? FullName { get; private set; }
        public string? Phone { get; private set; }
        public string? PhotoUrl { get; private set; }

        public UserRole RoleMask { get; private set; }

        public AccountStatus AccountStatus { get; private set; } = AccountStatus.Active;
        public DateTimeOffset? SuspendedTime { get; private set; }

        public bool IsProfileComplete { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? LastLoginAt { get; private set; }

        private User() { }

        public User(Guid UserId, UserRole Role, DateTimeOffset CreatedAtUtc)
        {
            if (UserId == Guid.Empty)
                throw new ArgumentException("UserID cannot be empty.");

            UserID = UserId;
            CreatedAt = CreatedAtUtc;

            RoleMask = UserRole.None;
            AddRoles(Role);

            IsProfileComplete = false;
        }

    // -------------------------
    //       Domain Layer
    // -------------------------
        public void UpdateProfile(string? email, string? phone, string? fullName, string? photoUrl)
        {
            Email = Normalize(email)?.ToLowerInvariant();
            Phone = Normalize(phone);
            FullName = Normalize(fullName);
            PhotoUrl = Normalize(photoUrl);

            if (Email is not null && Email.Length > 255)
                throw new ArgumentException("Email is too long.");

            if (Phone is not null && Phone.Length > 16)
                throw new ArgumentException("Phone is too long.");

            if (FullName is not null && FullName.Length > 150)
                throw new ArgumentException("FullName is too long.");

            if (PhotoUrl is not null && PhotoUrl.Length > 500)
                throw new ArgumentException("PhotoUrl is too long.");
        }

        // -------------------------
        //         Public ID 
        // -------------------------
        public void SetPublicUserId(string PublicUserId) 
        {
            PublicUserId = Normalize(PublicUserId) ?? throw new ArgumentException("PublicUserId cannot be empty.");
            if (PublicUserId.Length > 30)
                throw new ArgumentException("PublicUserId is too long.");

            PublicUserID = PublicUserId;
        }

        public void SetLastLogin(DateTimeOffset utcNow) => LastLoginAt = utcNow;

        // -------------------------
        //   Roles (bitmask rules)
        // -------------------------

        public bool HasRole(UserRole role) => (RoleMask & role) == role; // تأكد من وجود Role

        public void AddRoles(UserRole roles) // إضافة عدة أدوار للحساب
        {
            foreach (var role in SplitRoleMask(roles))
                AddSingleRole(role);
        }

        public void AddRole(UserRole role) => AddRoles(role); // إضافة دور واحد للحساب أسرع

        public void RemoveRole(UserRole role) // حذف رول من الحساب
        {
            if ((role & UserRole.Customer) == UserRole.Customer && HasRole(UserRole.Driver))
                throw new InvalidOperationException("Cannot remove Customer role from a Driver.");

            RoleMask &= ~role;
        }

        private void AddSingleRole(UserRole role) // عملبة الإضافة دور واحد
        {
            if (role == UserRole.None) return;

            if (role == UserRole.Driver)
                RoleMask |= UserRole.Customer;

            RoleMask |= role;
        }

        private static IEnumerable<UserRole> SplitRoleMask(UserRole roles) // فصل الأدوار عن بعضها
        {
            foreach (UserRole r in Enum.GetValues(typeof(UserRole)))
            {
                if (r == UserRole.None) continue;
                if ((roles & r) == r) yield return r;
            }
        }

        // -------------------------
        //      Status changes
        // -------------------------

        public void Activate() // تفعل الحساب
        {
            AccountStatus = AccountStatus.Active;
            SuspendedTime = null;
        }

        public void Suspend(DateTimeOffset? untilUtc) // تعليق الحساب (دائم أو مؤقت)د
        {
            AccountStatus = AccountStatus.Suspended;
            SuspendedTime = untilUtc;
        }

        public void Ban() // حظر دائم للحساب
        {
            AccountStatus = AccountStatus.Banned;
            SuspendedTime = null;
        }

        public bool IsSuspensionExpired(DateTimeOffset utcNow) // بتفحص التعليق إذا نتهت مدته ولا لاء
            => AccountStatus == AccountStatus.Suspended && SuspendedTime.HasValue && utcNow >= SuspendedTime.Value;

        // -------------------------
        //     Profile complete
        // -------------------------
        public void MarkProfileComplete(bool isComplete) => IsProfileComplete = isComplete; // تحدد إذا الحساب كتمل

        private static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim(); // تنظيف النص قبل التخزين
    }
}