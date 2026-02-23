using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.EntitiesTemp
{
    public class Notification
    {
        [Key]
        public Guid NotificationID { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(1000)]
        public string Body { get; set; } = string.Empty;

        [Required]
        public int Type { get; set; }

        public int? RelatedEntityType { get; set; }

        public Guid? RelatedEntityID { get; set; }

        [Required]
        public bool IsRead { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Notification()
        {
            NotificationID = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
            IsRead = false;
        }
    }
}