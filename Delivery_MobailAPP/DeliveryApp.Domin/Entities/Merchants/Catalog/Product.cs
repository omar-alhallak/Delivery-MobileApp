using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.Entities.Merchants.Catalog
{
    public class Product
    {
        public ProductID ID { get; private set; }

        public MerchantCategoryID MerchantCategoryID { get; private set; }

        public CatalogName ProductName { get; private set; } = null!;

        public string? Description { get; private set; }
        public string? ImageURL { get; private set; }

        public decimal? BasePrice { get; private set; }

        public bool IsActive { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private Product() { }

        public Product(ProductID id, MerchantCategoryID MerchantCategoryId, string productName, string? description,
            string? imageUrl, DateTimeOffset CreatedAtUtc, decimal? basePrice = null)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ID));

            if (MerchantCategoryId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(MerchantCategoryID));

            ID = id;
            MerchantCategoryID = MerchantCategoryId;

            SetName(productName);
            SetDescription(description);
            SetImageUrl(imageUrl);
            SetBasePrice(basePrice);

            IsActive = true;
            CreatedAt = CreatedAtUtc;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void Rename(string name) => SetName(name);

        public void ChangeDescription(string? description) => SetDescription(description);

        public void ChangeImage(string? imageUrl) => SetImageUrl(imageUrl);

        public void ChangeBasePrice(decimal? basePrice) => SetBasePrice(basePrice);

        public void MoveToCategory(MerchantCategoryID newMerchantCategoryId)
        {
            if (newMerchantCategoryId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(MerchantCategoryID));

            MerchantCategoryID = newMerchantCategoryId;
        }

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        // -------------------------
        //           Setters
        // -------------------------

        private void SetName(string value) =>
            ProductName = CatalogName.Create(value, maxLength: 150, fieldName: nameof(ProductName));

        private void SetDescription(string? value)
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 1000) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Description));

            Description = value;
        }

        private void SetImageUrl(string? value)
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(ImageURL));

            ImageURL = value;
        }

        private void SetBasePrice(decimal? value)
        {
            if (value.HasValue && value.Value <= 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(BasePrice));

            BasePrice = value;
        }

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}