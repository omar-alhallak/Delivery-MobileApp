using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.EntitiesTemp
{
    public class Merchant
    {
        [Key]
        public Guid MerchantID { get; set; }

        [Required]
        [MaxLength(100)]
        public string MerchantType { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string MerchantName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? LogoURL { get; set; }

        [MaxLength(500)]
        public string? CoverImageURL { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Lat { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Lng { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Merchant()
        {
            MerchantID = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;
        }
    }
}