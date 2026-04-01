using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.IdentityEnums;
using DeliveryApp.Domain.Enums.EngagementEnums;
using DeliveryApp.Domain.DomainErrors.IdentityErrors;

namespace DeliveryApp.Domain.Entities.Identity
{
    public class User // يمثل المستخدمين داخل النظام
    {
        // -------------------------
        //            Key
        // -------------------------

        public UserID ID { get; private set; } // PK معرف المستخدم
        public PublicCode? PublicID { get; private set; } // الكود الي بيظهر للمستخدم

        // -------------------------
        //     Personal Information
        // -------------------------

        public string? Email { get; private set; } // البريد الإلكتروني
        public string? FullName { get; private set; } // الاسم الكامل
        public string? Phone { get; private set; } // رقم الهاتف
        public string? PhotoUrl { get; private set; } // رابط الصورة الشخصية

        public bool IsProfileComplete { get; private set; } // هل الملف الشخصي مكتمل

        // -------------------------
        //          Roles
        // -------------------------

        public UserRole RoleMask { get; private set; } // أدوار المستخدم داخل النظام

        // -------------------------
        //          Status
        // -------------------------

        public AccountStatus AccountStatus { get; private set; } = AccountStatus.Active; // حالة الحساب
        public DateTimeOffset? SuspendedUntilUtc { get; private set; } // وقت انتهاء الإيقاف المؤقت إن وجد

        // -------------------------
        //          Rating
        // -------------------------

        public decimal CustomerAverageRating { get; private set; } // متوسط تقييم المستخدم كزبون
        public int CustomerRatingsCount { get; private set; } // عدد تقييمات المستخدم كزبون

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء الحساب
        public DateTimeOffset? LastLoginAt { get; private set; } // آخر وقت تسجيل دخول

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

        public void AssignPublicID(PublicCode publicId) // تعيين الكود العام
        {
            if (PublicID is not null) throw new DomainConflictException
                    (UserErrors.PublicIdAlreadyAssignedCode, UserErrors.PublicIdAlreadyAssignedMessage);

            PublicID = publicId;
        }

        public void UpdateProfile(string? email, string? phone, string? fullName, string? photoUrl) // تحديث بيانات الملف الشخصي
        {
            PreventModificationIfBanned();

            Email = Normalize(email)?.ToLowerInvariant();
            Phone = Normalize(phone);
            FullName = Normalize(fullName);
            PhotoUrl = Normalize(photoUrl);

            ValidateFieldLengths();

            if (IsProfileComplete && !HasRequiredProfileFields()) throw new DomainRuleViolationException
                    (UserErrors.CantRemoveRequiredFieldCode, UserErrors.CantRemoveRequiredFieldMessage);
        }

        public void MarkProfileAsComplete() // جعل الملف الشخصي مكتمل
        {
            PreventModificationIfBanned();

            if (!HasRequiredProfileFields()) throw new DomainRuleViolationException
                    (UserErrors.ProfileFieldNotCompleteCode, UserErrors.ProfileFieldNotCompleteMessage);

            IsProfileComplete = true;
        }

        public void MarkProfileAsIncomplete() // إزالة حالة اكتمال من الملف الشخصي
        {
            PreventModificationIfBanned();
            IsProfileComplete = false;
        }

        private bool HasRequiredProfileFields() => Phone is not null && FullName is not null; // التحقق من وجود الحقول المطلوبة لإكمال الملف الشخصي

        private void ValidateFieldLengths() // التحقق من أطوال الحقول
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
        //          Roles
        // -------------------------

        public bool HasRole(UserRole role) => (RoleMask & role) == role; // التحقق هل المستخدم يملك دور معين

        public void AddRoles(UserRole roles) // إضافة دور أو أكثر للمستخدم
        {
            PreventModificationIfBanned();

            foreach (var role in SplitRoleMask(roles))
            {
                AddSingleRole(role);
            }
        }

        public void RemoveRole(UserRole role) // إزالة دور من المستخدم
        {
            PreventModificationIfBanned();

            if (role == UserRole.None) return;

            if ((role & UserRole.Customer) == UserRole.Customer && HasRole(UserRole.Driver)) throw new DomainRuleViolationException
                    (UserErrors.CantRemoveCustFromDrivCode, UserErrors.CantRemoveCustFromDrivMessage);

            RoleMask &= ~role;

            CheckDriverIsCustomer();
        }

        private void AddSingleRole(UserRole role) // إضافة دور واحد
        {
            if (role == UserRole.None) return;

            if (role == UserRole.Driver)
                RoleMask |= UserRole.Customer;

            RoleMask |= role;

            CheckDriverIsCustomer();
        }

        private void CheckDriverIsCustomer() // التأكد أن السائق يملك دور الزبون
        {
            if (HasRole(UserRole.Driver) && !HasRole(UserRole.Customer))
                RoleMask |= UserRole.Customer;
        }

        private static IEnumerable<UserRole> SplitRoleMask(UserRole roles) // تقسيم الـ                                                                         
        {                                                                  // RoleMask إلى عدة أدوار
            foreach (UserRole r in Enum.GetValues(typeof(UserRole)))
            {
                if (r == UserRole.None) continue;
                if ((roles & r) == r) yield return r;
            }
        }

        // -------------------------
        //          Status
        // -------------------------

        public void Activate() // تفعيل الحساب
        {
            AccountStatus = AccountStatus.Active;
            SuspendedUntilUtc = null;
        }

        public void Suspend(DateTimeOffset? untilUtc, DateTimeOffset utcNow) // إيقاف الحساب مؤقتاً
        {
            PreventModificationIfBanned();

            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            if (untilUtc.HasValue && untilUtc <= utcNow) throw new DomainValidationException
                    (UserErrors.SuspensionMustBeFutureCode, UserErrors.SuspensionMustBeFutureMessage, nameof(untilUtc));

            AccountStatus = AccountStatus.Suspended;
            SuspendedUntilUtc = untilUtc;
        }

        public void Ban() // حظر الحساب نهائياً
        {
            AccountStatus = AccountStatus.Banned;
            SuspendedUntilUtc = null;
        }

        public bool IsSuspensionExpired(DateTimeOffset utcNow) // التحقق من انتهاء مدة الإيقاف المؤقت
        {
            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            return AccountStatus == AccountStatus.Suspended && SuspendedUntilUtc.HasValue && utcNow >= SuspendedUntilUtc.Value;
        }

        public void AutoActivateIfExpired(DateTimeOffset utcNow) // إعادة التفعيل تلقائياً عند انتهاء مدة الإيقاف
        {
            if (IsSuspensionExpired(utcNow)) Activate();
        }

        public void SetLastLogin(DateTimeOffset utcNow) // تحديث آخر وقت تسجيل دخول
        {
            if (utcNow == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(utcNow));

            if (utcNow < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(utcNow));

            LastLoginAt = utcNow;
        }

        private void PreventModificationIfBanned() // منع التعديل إذا كان الحساب محظور
        {
            if (AccountStatus == AccountStatus.Banned) throw new DomainRuleViolationException
                    (UserErrors.BannedCantBeModifiedCode, UserErrors.BannedCantBeModifiedMessage);
        }

        private static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim(); // تنظيف النصوص

        // -------------------------
        //          Rating
        // -------------------------

        public void AddCustomerRating(RatingStars stars) // إضافة تقييم جديد للمستخدم
        {
            ValidateRatingStars(stars);

            var value = (int)stars;

            CustomerAverageRating = ((CustomerAverageRating * CustomerRatingsCount) + value) / (CustomerRatingsCount + 1);
            CustomerRatingsCount++;
        }

        public void UpdateCustomerRating(RatingStars oldStars, RatingStars newStars) // تعديل تقييم
        {
            ValidateRatingStars(oldStars);
            ValidateRatingStars(newStars);

            if (CustomerRatingsCount <= 0) throw new DomainRuleViolationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage);

            var oldValue = (int)oldStars;
            var newValue = (int)newStars;

            CustomerAverageRating = ((CustomerAverageRating * CustomerRatingsCount) - oldValue + newValue) / CustomerRatingsCount;
        }

        private static void ValidateRatingStars(RatingStars stars) // التحقق من صحة التقييم (النجوم)د
        {
            if (!Enum.IsDefined(typeof(RatingStars), stars)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(stars));
        }
    }
}