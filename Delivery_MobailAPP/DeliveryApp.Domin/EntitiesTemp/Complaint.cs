using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.EntitiesTemp
{
    public class Complaint
    {
        [Key]
        public Guid ComplaintID { get; set; }


        [Required]
        public Guid CreatedByUserID { get; set; }

        [Required]
        public int TargetType { get; set; }

        [Required]
        public Guid TargetID { get; set; }


        public Guid? OrderID { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Message { get; set; }

        [Required]
        public int Status { get; set; }

        public Guid? ReviewedByAdminId { get; set; }

        [MaxLength(2000)]
        public string? AdminResponse { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? ResolvedAt { get; set; }

        public Complaint()
        {
            ComplaintID = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
            Status = 0;
        }
    }
}