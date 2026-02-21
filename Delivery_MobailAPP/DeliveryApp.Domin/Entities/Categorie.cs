using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace DeliveryApp.Domain.Entities
{
    internal class Categorie
    {

        public Guid CategoryId { get; private set; }


        public MerchantType MerchantType { get; private set; }



        public string ?CategoryName { get; private set; }


        public string ?Description { get; private set; }


        public enum Staut
        {
            Active = 5,
            Inactive = 2,
            OutOfStock = 3,
            Archived = 4
        }
        public Staut SortOrder { get; private set; }


        public string ?ImageURL { get; private set; }


        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private Categorie() { }

        public Categorie(string name, MerchantType merchantType, string? description = null, string? imageUrl = null)
        {

            CategoryId = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;

            UpdateDetails(name, merchantType, description, imageUrl);
        }

        public void UpdateDetails(string name, MerchantType merchantType, string? description, string? imageUrl)
        {
            if (Description is not null && Description.Length > 200)
                throw new ArgumentException("Description is too long.");

            if (CategoryName is not null && CategoryName.Length > 40)
                throw new ArgumentException("CategoryName is too long.");

            if (ImageURL is not null && ImageURL.Length > 500)
                throw new ArgumentException("ImageURL is too long.");


            CategoryName = name.Trim();
            MerchantType = merchantType;
            Description = description?.Trim();
            ImageURL = imageUrl?.Trim();
        }

        public void SetStatus(bool isActive) => IsActive = isActive;
    }


}

