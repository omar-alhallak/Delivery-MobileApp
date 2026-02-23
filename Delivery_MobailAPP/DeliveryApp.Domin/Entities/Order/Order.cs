using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities.Order
{
    public class Order
    {
        public Guid OrderID { get; set; }

        public int OrderType { get; set; }         

        public Guid CustomerID { get; set; }     
        public Guid? MerchantID { get; set; }    

        public decimal PickupLat { get; set; }
        public decimal PickupLng { get; set; }
        public decimal DropoffLat { get; set; }
        public decimal DropoffLng { get; set; }

        public decimal DistanceKmSnapshot { get; set; }
        public decimal ItemsTotalSnapshot { get; set; }
        public decimal DeliveryFeeSnapshot { get; set; }
        public decimal TipAmountSnapshot { get; set; }
        public decimal TotalAmountSnapshot { get; set; }

        public int PaymentMethod { get; set; }    
        public int PaymentStatus { get; set; }    

        public int Status { get; set; }   
        public short RequiredDriversCount { get; set; }  

        public int? FailureReason { get; set; }    

        public int? CancelledByType { get; set; }   
        public Guid? CancelledById { get; set; }   
        public DateTimeOffset? CancelledAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ConfirmedAt { get; set; }
        public DateTimeOffset? DeliveredAt { get; set; }

        public Order()
        {
            OrderID = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;

            Status = 0;            
            PaymentMethod = 0;   
            PaymentStatus = 0;    
            RequiredDriversCount = 1;
        }
    }
}