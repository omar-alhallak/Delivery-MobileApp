using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.EntitiesTemp
{
    public class MerchantUser
    {
        [Key, Column(Order = 0)]
        public Guid MerchantID { get; set; }

        [Key, Column(Order = 1)]
        public Guid UserID { get; set; }

        public int Role { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public MerchantUser()
        {
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;
        }
    }
}