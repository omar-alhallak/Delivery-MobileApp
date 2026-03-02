using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryApp.Domain.ValueObjects;

namespace DeliveryApp.Domain.Entities.Merchants
{
    public class Merchant
    {

        public  MerchantID ID { get; private set; }

        public PublicCode? PublicID { get; private set; }
        public string MerchantType { get; private set; } = string.Empty;

        public string MerchantName { get; private set; } = string.Empty;

        public string? Description { get; private set; }

        public string? Phone { get; private set; }

        public string? LogoURL { get; private set; }

        public string? CoverImageURL { get; private set; }

        public decimal Lat { get; private set; }

        public decimal Lng { get; private set; }

        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private Merchant() { }

        public Merchant(MerchantID id, string merchantType, string merchantName, string? description, string? phone, string? logoURL, string? coverImageURL, decimal lat, decimal lng)
        {
            ID = id;
            MerchantType = merchantType;
            MerchantName = merchantName;
            Description = description;
            Phone = phone;
            LogoURL = logoURL;
            CoverImageURL = coverImageURL;
            Lat = lat;
            Lng = lng;
            IsActive = true;
            CreatedAt = DateTimeOffset.UtcNow;
            UpdateMerchant(merchantType, merchantName, description, phone, logoURL, coverImageURL, lat, lng);
        }
        public void UpdateMerchant(string merchantType, string merchantName, string? description, string? phone, string? logoURL, string? coverImageURL, decimal lat, decimal lng)
        {
            MerchantType = Normalize(merchantType) ?? string.Empty; 
            MerchantName = Normalize(merchantName) ?? string.Empty;
            Description = Normalize(description);
            Phone = Normalize(phone);
            LogoURL = Normalize(logoURL);
            CoverImageURL = Normalize(coverImageURL);
            Lat = lat;
            Lng = lng;
            FieldLimits();
            ProfileCompletionRules();
            if (IsActive && !MerchantCompletionRules())
                throw new InvalidOperationException("Profile is complete, required fields cannot be cleared.");

        }
        private bool ProfileCompletionRules() => Phone is not null && MerchantName is not null;
        private void FieldLimits()
        {
            if (MerchantName is not null && MerchantName.Length > 150)
                throw new ArgumentException("MerchantName too long.");

            if (Phone is not null && Phone.Length > 16)
                throw new ArgumentException("The phone number is too long.");

            if (LogoURL is not null && LogoURL.Length > 500)
                throw new ArgumentException("PhotoUrl too long.");

            if (Phone is not null && Phone.Length > 16)
                throw new ArgumentException("Phone too long.");

            if (Description?.Length > 500)
                throw new ArgumentException("The description is very long.");

            if (Lat < -90 || Lat > 90)
                throw new ArgumentException("Incorrect latitude coordinates.");

            if (Lng < -180 || Lng > 180)
                throw new ArgumentException("Incorrect longitude coordinates.");

            if (Lat == 0 && Lng == 0)
                throw new ArgumentException("The merchant's location on the map must be precisely determined.");
            if (LogoURL?.Length > 500)
                throw new ArgumentException("The logo URL is very long.");
            if (CoverImageURL?.Length > 500)
                throw new ArgumentException("The cover image URL is very long.");
            if (MerchantType.Length > 50)
                throw new ArgumentException("The merchant type is very long.");
        }
        public void ProfileComplete()
        {
            PreventModificationIfInactive();

            if (!MerchantCompletionRules())
                throw new InvalidOperationException("Store name, phone number, logo, description, and location on the map are required fields.");

            IsActive = true; 
        }
        public void ProfileNotComplete()
        {
            PreventModificationIfInactive();
            IsActive = false; 
        }
        private bool MerchantCompletionRules() =>
            !string.IsNullOrWhiteSpace(MerchantName) &&
            !string.IsNullOrWhiteSpace(Phone) &&
            !string.IsNullOrWhiteSpace(LogoURL) &&
            Lat != 0 && Lng != 0;
       
        public void Activate()
        {
            IsActive = true;
        }
        public void AssignPublicID(PublicCode publicId)
        {
            if (PublicID is not null)
                throw new InvalidOperationException("PublicID already assigned.");

            PublicID = publicId;
        }
        private void PreventModificationIfInactive()
        {
            if (!IsActive)
                throw new InvalidOperationException("Inactive merchant data cannot be modified.");
        }
        private static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }
}