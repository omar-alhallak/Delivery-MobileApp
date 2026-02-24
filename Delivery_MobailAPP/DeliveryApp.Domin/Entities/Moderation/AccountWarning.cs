using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Moderation
{
    public class AccountWarning
    {
        [Key]
        public Guid WarningID { get; set; }

        [Required]
        public int EntityType { get; set; }  

        [Required]
        public Guid EntityID { get; set; }

        public Guid? RelatedOrderID { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public int Severity { get; set; }  

        [Required]
        public Guid CreatedByAdminId { get; set; }

        public int? Decision { get; set; }   

        public DateTimeOffset? ExpiresAt { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? DecidedAt { get; set; }

        public AccountWarning()
        {
            WarningID = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}