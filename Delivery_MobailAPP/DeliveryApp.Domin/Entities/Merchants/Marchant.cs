using System;
using DeliveryApp.Domain.Enums;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.MerchantErrors;
using DeliveryApp.Domain.Enums.EngagementEnams;

namespace DeliveryApp.Domain.Entities.Merchants
{
    public class Merchant
    {
        public MerchantID ID { get; private set; }
        public PublicCode? PublicID { get; private set; }

        public MerchantType MerchantType { get; private set; }

        public string MerchantName { get; private set; } = null!;
        public Slug Slug { get; private set; } = null!;

        public string? Description { get; private set; }
        public string? Phone { get; private set; }

        public string? LogoUrl { get; private set; }
        public string? CoverImageUrl { get; private set; }

        public GeoPoint Location { get; private set; } = null!;

        public decimal AverageRating { get; private set; }
        public int RatingsCount { get; private set; }

        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private Merchant() { }

        public Merchant(MerchantID id, MerchantType merchantType, string merchantName, string slug, decimal lat, decimal lng,
            string? description, string? phone, string? logoUrl, string? coverImageUrl, DateTimeOffset CreatedAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (CreatedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(CreatedAtUtc));

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

            CreatedAt = CreatedAtUtc;
            IsActive = false;
        }

        // ------------ Public ID ------------
        public void AssignPublicID(PublicCode publicId)
        {
            if (PublicID is not null) throw new DomainConflictException
                    (MerchantErrors.PublicIdAlreadyAssignedCode, MerchantErrors.PublicIdAlreadyAssignedMessage);

            PublicID = publicId;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void Rename(string name) => SetName(name);

        public void ChangeSlug(string slug) => SetSlug(slug);

        public void ChangeDescription(string? description) => SetDescription(description);

        public void ChangePhone(string? phone) => SetPhone(phone);

        public void ChangeLogo(string? logoUrl) => SetLogoUrl(logoUrl);

        public void ChangeCoverImage(string? coverImageUrl) => SetCoverImageUrl(coverImageUrl);

        public void Relocate(decimal lat, decimal lng) => SetLocation(lat, lng);

        public void Deactivate()
        {
            if (!IsActive) return;

            IsActive = false;
        }

        public void Activate()
        {
            if (IsActive) return;

            CheckCanBeActivated();
            IsActive = true;
        }

        public void AddRating(RatingStars stars)
        {
            ValidateRatingStars(stars);

            var value = (int)stars;

            AverageRating = ((AverageRating * RatingsCount) + value) / (RatingsCount + 1);
            RatingsCount++;
        }

        public void UpdateRating(RatingStars oldStars, RatingStars newStars)
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

        private void CheckCanBeActivated()
        {
            if (string.IsNullOrWhiteSpace(MerchantName)) throw new DomainRuleViolationException
                    (MerchantErrors.CantActivateWithoutNameCode, MerchantErrors.CantActivateWithoutNameMessage);

            if (string.IsNullOrWhiteSpace(LogoUrl)) throw new DomainRuleViolationException
                    (MerchantErrors.CantActivateWithoutLogoCode, MerchantErrors.CantActivateWithoutLogoMessage);

            if (string.IsNullOrWhiteSpace(CoverImageUrl)) throw new DomainRuleViolationException
                    (MerchantErrors.CantActivateWithoutCoverImageCode, MerchantErrors.CantActivateWithoutCoverImageMessage);
        }

        private static void ValidateRatingStars(RatingStars stars)
        {
            if (!Enum.IsDefined(typeof(RatingStars), stars)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(stars));
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(MerchantName));

            value = value.Trim();

            if (value.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(MerchantName));

            MerchantName = value;
        }

        private void SetSlug(string value) => Slug = Slug.Create(value);

        private void SetLocation(decimal lat, decimal lng) => Location = GeoPoint.Create(lat, lng);

        private void SetDescription(string? value)
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 2000) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Description));

            Description = value;
        }

        private void SetPhone(string? value)
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 20) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Phone));

            Phone = value;
        }

        private void SetLogoUrl(string? value)
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(LogoUrl));

            LogoUrl = value;
        }

        private void SetCoverImageUrl(string? value)
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(CoverImageUrl));

            CoverImageUrl = value;
        }

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}