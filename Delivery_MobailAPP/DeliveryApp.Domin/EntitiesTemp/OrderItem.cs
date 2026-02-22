using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.EntitiesTemp
{
    public class OrderItem
    {
        [Key]
        public Guid OrderItemID { get; set; }

        [Required]
        public Guid OrderID { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProductNameSnapshot { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? VariantNameSnapshot { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPriceSnapshot { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotalSnapshot { get; set; }

        [MaxLength(500)]
        public string? CustomerNotes { get; set; }

        public OrderItem()
        {
            OrderItemID = Guid.NewGuid();
        }
    }
}