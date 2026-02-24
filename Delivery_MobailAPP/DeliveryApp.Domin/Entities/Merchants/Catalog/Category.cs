using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace DeliveryApp.Domain.Entities.Merchants.Catalog
{
    public class Category
    {
        [Key]
        public Guid CategorieID { get; set; }

        [Required]
        [MaxLength(100)]
        public string MerchantType { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string CategoriesName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageURL { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Category()
        {
            CategorieID = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;
        }
    }
}