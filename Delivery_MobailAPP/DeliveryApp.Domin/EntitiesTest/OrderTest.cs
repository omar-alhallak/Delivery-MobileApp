using DeliveryApp.Domain.Enums.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.Entities
{
    public class OrderTest
    {
        public Guid OrderID { get; private set; }

        public OrderType OrderType { get; private set; }
        public Guid CustomerID { get; private set; }
        public Guid? MerchantID { get; private set; } 

        public decimal PickupLat { get; private set; }
        public decimal PickupLng { get; private set; }
        public decimal DropoffLat { get; private set; }
        public decimal DropoffLng { get; private set; }

        public decimal DistanceKmSnapshot { get; private set; }

        public decimal ItemsTotalSnapshot { get; private set; }
        public decimal DeliveryFeeSnapshot { get; private set; }
        public decimal TipAmountSnapshot { get; private set; }
        public decimal TotalAmountSnapshot { get; private set; }

        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }

        public OrderStatus Status { get; private set; }

        public int RequiredDriversCount { get; private set; }
         public int AcceptedDriversCount { get; private set; } 

        public OrderIssueReason IssueReason { get; private set; } = OrderIssueReason.None;
         public string? IssueNotes { get; private set; }

        public CancelledByType CancelledByType { get; private set; } = CancelledByType.None;
        public Guid? CancelledById { get; private set; }
        public DateTimeOffset? CancelledAt { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? ConfirmedAt { get; private set; }
        public DateTimeOffset? DeliveredAt { get; private set; }

        private OrderTest() { }

        private OrderTest(Guid OrderId, OrderType OrderType, Guid CustomerId, Guid? MerchantId, decimal pickupLat,
            decimal pickupLng, decimal dropoffLat, decimal dropoffLng, decimal distanceKmSnapshot, decimal itemsTotalSnapshot,
            decimal deliveryFeeSnapshot, decimal tipAmountSnapshot, PaymentMethod paymentMethod, int requiredDriversCount, DateTimeOffset CreatedAtUtc)
        {
            if (OrderId == Guid.Empty) throw new ArgumentException("OrderId cannot be empty.");
            if (CustomerId == Guid.Empty) throw new ArgumentException("CustomerId cannot be empty.");

            if (OrderType == OrderType.SuperMarket && (MerchantId is null || MerchantId == Guid.Empty))
                throw new ArgumentException("MerchantId is required for SuperMarket orders.");

            if (OrderType == OrderType.Restaurant && MerchantId is not null)
                throw new ArgumentException("MerchantId must be null for Restaurant orders.");

            if (distanceKmSnapshot < 0) throw new ArgumentException("Distance cannot be negative.");
            if (itemsTotalSnapshot < 0) throw new ArgumentException("ItemsTotal cannot be negative.");
            if (deliveryFeeSnapshot < 0) throw new ArgumentException("DeliveryFee cannot be negative.");
            if (tipAmountSnapshot < 0) throw new ArgumentException("Tip cannot be negative.");

            if (requiredDriversCount <= 0 || requiredDriversCount > 3)
                throw new ArgumentException("RequiredDriversCount must be between 1 and 3.");

            OrderID = OrderId;
            this.OrderType = OrderType;
            CustomerID = CustomerId;
            MerchantID = MerchantId;

            PickupLat = pickupLat;
            PickupLng = pickupLng;
            DropoffLat = dropoffLat;
            DropoffLng = dropoffLng;

            DistanceKmSnapshot = distanceKmSnapshot;

            ItemsTotalSnapshot = itemsTotalSnapshot;
            DeliveryFeeSnapshot = deliveryFeeSnapshot;
            TipAmountSnapshot = tipAmountSnapshot;

            TotalAmountSnapshot = CalculateTotal(itemsTotalSnapshot, deliveryFeeSnapshot, tipAmountSnapshot);

            PaymentMethod = paymentMethod;
            PaymentStatus = PaymentStatus.UnPaid;

            Status = OrderStatus.Draft;

            RequiredDriversCount = requiredDriversCount;
            AcceptedDriversCount = 0;

            CreatedAt = CreatedAtUtc;
        }

        public static OrderTest CreateSuperMarketOrder(Guid OrderId, Guid CustomerId, Guid MerchantId, decimal pickupLat, decimal pickupLng,
            decimal dropoffLat, decimal dropoffLng, decimal distanceKmSnapshot, decimal itemsTotalSnapshot, decimal deliveryFeeSnapshot,
            decimal tipAmountSnapshot, int requiredDriversCount, DateTimeOffset createdAtUtc)
        {
            return new OrderTest(OrderId, OrderType.SuperMarket, CustomerId, MerchantId, pickupLat, pickupLng, dropoffLat, dropoffLng,
                distanceKmSnapshot, itemsTotalSnapshot, deliveryFeeSnapshot, tipAmountSnapshot, PaymentMethod.Cash,
                requiredDriversCount, createdAtUtc);
        }

        public static OrderTest CreateRestaurantOrder(Guid OrderId, Guid CustomerId, decimal pickupLat, decimal pickupLng, decimal dropoffLat,
            decimal dropoffLng, decimal distanceKmSnapshot, decimal deliveryFeeSnapshot, decimal tipAmountSnapshot,
            int requiredDriversCount, DateTimeOffset CreatedAtUtc)
        {
            return new OrderTest(OrderId, OrderType.Restaurant, CustomerId, MerchantId: null, pickupLat, pickupLng,
                dropoffLat, dropoffLng, distanceKmSnapshot, itemsTotalSnapshot: 0m, deliveryFeeSnapshot, tipAmountSnapshot,
                PaymentMethod.Cash, requiredDriversCount, CreatedAtUtc);
        }

        public void Confirm(DateTimeOffset UtcNow) // تأكيد الطلب
        {
            EnsureStatus(OrderStatus.Draft);
            Status = OrderStatus.SearchingDrivers;
            ConfirmedAt = UtcNow;
        }

        public void IncrementAcceptedDrivers() 
        {
            if (Status != OrderStatus.SearchingDrivers && Status != OrderStatus.DriversConfirmed)
                throw new InvalidOperationException("Drivers can only be accepted while searching/confirming.");

            if (AcceptedDriversCount >= RequiredDriversCount)
                throw new InvalidOperationException("AcceptedDriversCount cannot exceed RequiredDriversCount.");

            AcceptedDriversCount++;

            if (AcceptedDriversCount == RequiredDriversCount)
            {
                Status = OrderStatus.DriversConfirmed;

                if (OrderType == OrderType.SuperMarket)
                    Status = OrderStatus.AwaitingMerchantApproval;
            }
        }

        public void MerchantApprove() // يتأكد أنو التاجر قبل الطلب لينققل للتحضير
        {
            EnsureVendorOrder();
            EnsureStatus(OrderStatus.AwaitingMerchantApproval);
            Status = OrderStatus.Preparing;
        }

        public void MerchantReject(OrderIssueReason reason, string? notes = null) // حالة الرفض
        {
            EnsureVendorOrder();
            EnsureStatus(OrderStatus.AwaitingMerchantApproval);

            SetIssue(reason, notes);
            CancelInternal(CancelledByType.Merchant, cancelledById: null, DateTimeOffset.UtcNow);
        }

        public void MarkReadyForPickup() 
        {
            EnsureVendorOrder();
            EnsureStatus(OrderStatus.Preparing);
            Status = OrderStatus.ReadyForPickup;
        }

        public void MarkPickedUp()
        {
            if (OrderType == OrderType.SuperMarket)
                EnsureStatus(OrderStatus.ReadyForPickup);
            else
                EnsureStatus(OrderStatus.DriversConfirmed);

            Status = OrderStatus.PickedUp;
        }

        public void StartDelivery()
        {
            EnsureStatus(OrderStatus.PickedUp);
            Status = OrderStatus.OnTheWay;
        }

        public void Deliver(DateTimeOffset utcNow)
        {
            EnsureStatus(OrderStatus.OnTheWay);
            Status = OrderStatus.Delivered;
            DeliveredAt = utcNow;

            PaymentStatus = PaymentStatus.Paid;
        }

        public void FailDelivery(OrderIssueReason reason, string? notes = null)
        {
            if (Status != OrderStatus.OnTheWay)
                throw new InvalidOperationException("Delivery can only fail while OnTheWay.");

            SetIssue(reason, notes);
            Status = OrderStatus.DeliveryFailed;
        }

        public void Cancel(CancelledByType cancelledByType, Guid? cancelledById, OrderIssueReason reason, string? notes, DateTimeOffset utcNow)
        {
            if (Status == OrderStatus.PickedUp || Status == OrderStatus.OnTheWay || Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel after pickup/delivery. Requires admin decision.");

            SetIssue(reason, notes);
            CancelInternal(cancelledByType, cancelledById, utcNow);
        }

        private void CancelInternal(CancelledByType cancelledByType, Guid? cancelledById, DateTimeOffset utcNow)
        {
            Status = OrderStatus.Cancelled;
            CancelledByType = cancelledByType;
            CancelledById = cancelledById;
            CancelledAt = utcNow;
            PaymentStatus = PaymentStatus.UnPaid;
        }

        public void SetIssue(OrderIssueReason reason, string? notes = null)
        {
            IssueReason = reason;
            IssueNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            if (IssueNotes is not null && IssueNotes.Length > 500)
                throw new ArgumentException("IssueNotes is too long.");
        }

        private void EnsureStatus(OrderStatus required)
        {
            if (Status != required)
                throw new InvalidOperationException($"Order must be {required} but is {Status}.");
        }

        private void EnsureVendorOrder()
        {
            if (OrderType != OrderType.SuperMarket)
                throw new InvalidOperationException("This action is only valid for Vendor orders.");
        }

        private static decimal CalculateTotal(decimal items, decimal delivery, decimal tip) => items + delivery + tip;
    }
}