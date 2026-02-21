using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities
{
    internal class Product
    {

        public Guid ProductId { get; private set; }



        public Guid MerchantId { get; private set; }

        public Merchant Merchant { get; private set; } = null!;

        public Guid CategorieId { get; private set; }
        public Categorie? Categorie { get; private set; } = null!;


        public string ? ProductName { get; private set; }

        public decimal ? Price { get; private set; }


        public string ?Description { get; private set; }



        public string ?ImageURL { get; private set; }

        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }


        private Product() { }

        public Product(string name, decimal price, Guid categoryId, Guid merchantId, int stock = 0)
        {
            ProductId = Guid.NewGuid(); 
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;


            UpdateDetails(name, price, categoryId, merchantId, stock);
        }

        public void UpdateDetails(string name, decimal price, Guid categoryId, Guid merchantId, int stock)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");
            if (price <= 0) throw new ArgumentException("Price must be greater than zero");
            if (categoryId == Guid.Empty || merchantId == Guid.Empty) throw new ArgumentException("Invalid IDs");

            ProductName = name.Trim();
            Price = price;
            CategorieId = categoryId;
            MerchantId = merchantId;
        }
        public void UpdateDetails(string name, decimal price, int stock)
        {
            // 1. فاليديشن النصوص
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The product name cannot be empty.");

            // 2. فاليديشن الأسعار (الضمير المالي)
            if (price <= 0)
                throw new ArgumentException("The product price must be greater than zero.");

            // 3. فاليديشن المخزون
            if (stock < 0)
                throw new ArgumentException("Inventory cannot be negative");

            this.ProductName = name.Trim();
            this.Price = price;
        }

        public void SetDescription(string? desc) => Description = desc?.Trim();
        public void SetImageUrl(string? url) => ImageURL = url;
        public void SetStatus(bool isActive) => IsActive = isActive;
    }

}

