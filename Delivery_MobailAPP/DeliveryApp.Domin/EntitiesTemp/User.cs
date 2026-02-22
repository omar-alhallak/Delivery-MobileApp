using DeliveryApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.EntitiesTemp
{
    public class User
    {
        [Key]
        public Guid UserID { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(150)]
        public string? FullName { get; set; }

        [MaxLength(16)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? PhotoURL { get; set; }

        public UserRole RoleMask { get; set; }

        public int AccountStatus { get; set; }

        public bool IsProfileComplete { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? LastLoginAt { get; set; }

        public User()
        {
            UserID = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
            RoleMask = 0;
            AccountStatus = 1; 
            IsProfileComplete = false;
        }
    }
}