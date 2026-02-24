using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Merchants.Catalog
{
    public class Variant
    {
        [Key]
        public Guid VariantID { get; set; }

        [Required]
        public Guid ProductID { get; set; }

        [Required]
        [MaxLength(150)]
        public string VariantName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Variant()
        {
            VariantID = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;
        }
    }
}