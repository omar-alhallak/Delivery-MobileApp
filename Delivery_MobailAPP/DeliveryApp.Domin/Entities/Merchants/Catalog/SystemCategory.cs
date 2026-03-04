using System;
using DeliveryApp.Domain.Enums;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Merchants.Catalog
{
    public class SystemCategory
    {
        public SystemCategoryID ID { get; private set; }
        public MerchantType MerchantType { get; private set; }

        public string CategoriesName { get; private set; } = null!;
        public Slug Slug { get; private set; } = null!;

        public string? ImageURL { get; private set; }

        public int SortOrder { get; private set; }
        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private SystemCategory() { }

        public SystemCategory(SystemCategoryID id, MerchantType merchantType, string categoriesName
            , string slug, string? imageUrl, int sortOrder, DateTimeOffset CreatedAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: nameof(id));

            ID = id;
            MerchantType = merchantType;

            SetName(categoriesName);
            SetSlug(slug);

            SetImageUrl(imageUrl);
            SetSortOrder(sortOrder);

            IsActive = true;
            CreatedAt = CreatedAtUtc;
        }

        // -------------------------
        //          Behavior
        // -------------------------

        public void Rename(string categoriesName) => SetName(categoriesName);

        public void ChangeSlug(string slug) => SetSlug(slug);

        public void ChangeImage(string? imageUrl) => SetImageUrl(imageUrl);

        public void ChangeSortOrder(int sortOrder) => SetSortOrder(sortOrder);

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        // -------------------------
        //           setters
        // -------------------------

        private void SetName(string value)
        {
            value = NormalizeRequired(value, nameof(CategoriesName));

            if (value.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, field: nameof(CategoriesName));

            CategoriesName = value;
        }

        private void SetSlug(string value)
        {
            Slug = Slug.Create(value);
        }

        private void SetImageUrl(string? value)
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, field: nameof(ImageURL));

            ImageURL = value;
        }

        private void SetSortOrder(int value)
        {
            if (value < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, field: nameof(SortOrder));

            SortOrder = value;
        }

        // -------------------------
        //          Helpers
        // -------------------------

        private static string NormalizeRequired(string? value, string field)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, field: field);

            return value.Trim();
        }

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}