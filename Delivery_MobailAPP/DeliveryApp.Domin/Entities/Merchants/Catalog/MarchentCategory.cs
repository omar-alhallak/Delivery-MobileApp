using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Merchants.Catalog
{
    public class MerchantCategory
    {
        public MerchantCategoryID ID { get; private set; }
        public MerchantID MerchantID { get; private set; }

        public CatalogName CategoriesName { get; private set; } = null!;

        public string? Description { get; private set; }
        public string? ImageURL { get; private set; }

        public int SortOrder { get; private set; }
        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private MerchantCategory() { }

        public MerchantCategory(MerchantCategoryID id, MerchantID MerchantId, string categoriesName,
            string? description, string? imageUrl, int sortOrder, DateTimeOffset CreatedAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (MerchantId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(MerchantId));

            ID = id;
            MerchantID = MerchantId;

            SetName(categoriesName);
            SetDescription(description);
            SetImageUrl(imageUrl);
            SetSortOrder(sortOrder);

            IsActive = true;
            CreatedAt = CreatedAtUtc;
        }

        // -------------------------
        //          Behavior
        // -------------------------

        public void Rename(string name) => SetName(name);

        public void ChangeDescription(string? description) => SetDescription(description);

        public void ChangeImage(string? imageUrl) => SetImageUrl(imageUrl);

        public void ChangeSortOrder(int sortOrder) => SetSortOrder(sortOrder);

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        // -------------------------
        //          Setters
        // -------------------------

        private void SetName(string value)
        {
            CategoriesName = CatalogName.Create(value, 150, nameof(CategoriesName));
        }

        private void SetDescription(string? value)
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500)
                throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Description));

            Description = value;
        }

        private void SetImageUrl(string? value)
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500)
                throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(ImageURL));

            ImageURL = value;
        }

        private void SetSortOrder(int value)
        {
            if (value < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(SortOrder));

            SortOrder = value;
        }

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}