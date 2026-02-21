using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities
{
    public enum MerchantType
    {
        Restaurant = 1,
        Grocery = 2,
        Pharmacy = 3,
        Other = 4
    }

    internal class Merchant
    {


        public Guid MerchantId { get; private set; }



        public MerchantType? Type { get; private set; }


        public string ?MerchantName { get; private set; }


        public string ?Description { get; private set; }


        public string ?Phone {  get; private set; }

        public string ?LogoURL { get; private set; }

        public string ? CoverImageURL { get; private set; }

        public decimal Lat {  get; private set; }

        public decimal Long { get; private set; }

        
        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private Merchant() { }

        public Merchant(string name, string phone, MerchantType type, decimal lat, decimal lng)
        {

            MerchantId = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;

            UpdateDetails(name, phone, type, lat, lng);
        }

        public void UpdateDetails(string name, string phone, MerchantType type, decimal lat, decimal lng)
        {
            if (Phone is not null && Phone.Length > 16)
                throw new ArgumentException("Phone is too long.");

            if (MerchantName is not null && MerchantName.Length > 150)
                throw new ArgumentException("MerchantName is too long.");

            if (LogoURL is not null && LogoURL.Length > 500)
                throw new ArgumentException("LogoUrl is too long.");

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");

            MerchantName = name.Trim();
            Phone = phone.Trim();
            Type = type;
            Lat = lat;
            Long = lng;
        }

        public void SetImages(string? logo, string? cover)
        {
            LogoURL = logo;
            CoverImageURL = cover;
        }
        public void SetStatus(bool isActive) => IsActive = isActive;

    }

}

