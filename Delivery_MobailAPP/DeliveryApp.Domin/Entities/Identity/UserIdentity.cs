using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Identity
{
    public class UserIdentity
    {
        [Key]
        public Guid IdentiteID { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [MaxLength(50)]
        public string Provider { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? ProviderUserID { get; set; }

        [MaxLength(500)]
        public string? PasswordHash { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public UserIdentity()
        {
            IdentiteID = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}