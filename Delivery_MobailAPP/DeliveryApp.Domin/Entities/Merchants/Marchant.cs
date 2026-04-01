using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.MerchantEnums;
using DeliveryApp.Domain.Enums.EngagementEnums;
using DeliveryApp.Domain.DomainErrors.MerchantErrors;

namespace DeliveryApp.Domain.Entities.Merchants
{
    public class Merchant // يمثل التجار (مطاعم و متاجر)د
    {
        // -------------------------
        //            Key
        // -------------------------

        public MerchantID ID { get; private set; } // PK معرف التاجر
        public PublicCode? PublicID { get; private set; } // الكود العام الي بيظهر للمستخدم

        // -------------------------
        //        Basic Info
        // -------------------------

        public MerchantType MerchantType { get; private set; } // نوع التاجر
        public CatalogName MerchantName { get; private set; } = null!; // اسم التاجر
        public Slug Slug { get; private set; } = null!; // الاسم المختصر المستخدم في الروابط

        public string? Description { get; private set; } // وصف التاجر
        public string? Phone { get; private set; } // رقم هاتف التاجر

        public string? LogoUrl { get; private set; } // شعار التاجر
        public string? CoverImageUrl { get; private set; } // صورة الغلاف

        // -------------------------
        //         Location
        // -------------------------

        public GeoPoint Location { get; private set; } = null!; // موقع التاجر

        // -------------------------
        //          Rating
        // -------------------------

        public decimal AverageRating { get; private set; } // متوسط تقييم التاجر
        public int RatingsCount { get; private set; } // عدد تقييمات التاجر

        // -------------------------
        //          State
        // -------------------------

        public bool IsActive { get; private set; } // هل التاجر مفعل

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء التاجر

        private Merchant() { }

        public Merchant(MerchantID id, MerchantType merchantType, string merchantName, string slug, decimal lat, decimal lng, string? description,
            string? phone, string? logoUrl, string? coverImageUrl, DateTimeOffset createdAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            if (!Enum.IsDefined(typeof(MerchantType), merchantType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(merchantType));

            ID = id;
            MerchantType = merchantType;

            SetName(merchantName);
            SetSlug(slug);
            SetLocation(lat, lng);

            SetDescription(description);
            SetPhone(phone);

            SetLogoUrl(logoUrl);
            SetCoverImageUrl(coverImageUrl);

            AverageRating = 0;
            RatingsCount = 0;

            CreatedAt = createdAtUtc;
            IsActive = false;
        }

        // -------------------------
        //         Public ID
        // -------------------------

        public void AssignPublicID(PublicCode publicId) // تعيين الكود العام
        {
            if (PublicID is not null) throw new DomainConflictException
                    (MerchantErrors.PublicIdAlreadyAssignedCode, MerchantErrors.PublicIdAlreadyAssignedMessage);

            PublicID = publicId;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void Rename(string name) => SetName(name); // تغيير اسم التاجر

        public void ChangeSlug(string slug) => SetSlug(slug); // تغيير ال Slug

        public void ChangeDescription(string? description) => SetDescription(description); // تغيير وصف التاجر

        public void ChangePhone(string? phone) => SetPhone(phone); // تغيير رقم هاتف التاجر

        public void ChangeLogo(string? logoUrl) => SetLogoUrl(logoUrl); // تغيير شعار التاجر

        public void ChangeCoverImage(string? coverImageUrl) => SetCoverImageUrl(coverImageUrl); // تغيير صورة الغلاف

        public void Relocate(decimal lat, decimal lng) => SetLocation(lat, lng); // تغيير موقع التاجر

        public void Activate() // تفعيل التاجر
        {
            if (IsActive) return;

            EnsureCanBeActivated();
            IsActive = true;
        }

        public void Deactivate() // تعطيل التاجر
        {
            if (!IsActive) return;

            IsActive = false;
        }

        public void AddRating(RatingStars stars) // إضافة تقييم جديد للتاجر
        {
            ValidateRatingStars(stars);

            var value = (int)stars;

            AverageRating = ((AverageRating * RatingsCount) + value) / (RatingsCount + 1);
            RatingsCount++;
        }

        public void UpdateRating(RatingStars oldStars, RatingStars newStars) // تعديل تقييم موجود للتاجر
        {
            ValidateRatingStars(oldStars);
            ValidateRatingStars(newStars);

            if (RatingsCount <= 0) throw new DomainRuleViolationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage);

            var oldValue = (int)oldStars;
            var newValue = (int)newStars;

            AverageRating = ((AverageRating * RatingsCount) - oldValue + newValue) / RatingsCount;
        }

        // -------------------------
        //        Validation
        // -------------------------

        private void EnsureCanBeActivated() // التأكد أن التاجر يملك البيانات المطلوبة قبل التفعيل
        {
            if (string.IsNullOrWhiteSpace(MerchantName.Value)) throw new DomainRuleViolationException
                    (MerchantErrors.CantActivateWithoutNameCode, MerchantErrors.CantActivateWithoutNameMessage);

            if (string.IsNullOrWhiteSpace(LogoUrl)) throw new DomainRuleViolationException
                    (MerchantErrors.CantActivateWithoutLogoCode, MerchantErrors.CantActivateWithoutLogoMessage);

            if (string.IsNullOrWhiteSpace(CoverImageUrl)) throw new DomainRuleViolationException
                    (MerchantErrors.CantActivateWithoutCoverImageCode, MerchantErrors.CantActivateWithoutCoverImageMessage);
        }

        private static void ValidateRatingStars(RatingStars stars) // التحقق من صحة تقييم
        {
            if (!Enum.IsDefined(typeof(RatingStars), stars)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(stars));
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetName(string value) => MerchantName = CatalogName.Create(value, 150, nameof(MerchantName)); // إدخال اسم التاجر

        private void SetSlug(string value)  =>  Slug = Slug.Create(value); // إدخال ال Slug

        private void SetLocation(decimal lat, decimal lng) => Location = GeoPoint.Create(lat, lng); // إدخال موقع التاجر

        private void SetDescription(string? value) // إدخال وصف التاجر
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 2000) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Description));

            Description = value;
        }

        private void SetPhone(string? value) // إدخال رقم هاتف التاجر
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 20) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Phone));

            Phone = value;
        }

        private void SetLogoUrl(string? value) // إدخال شعار التاجر
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(LogoUrl));

            LogoUrl = value;
        }

        private void SetCoverImageUrl(string? value) // إدخال صورة الغلاف
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(CoverImageUrl));

            CoverImageUrl = value;
        }

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim(); // تنظيف النصوص
    }
}