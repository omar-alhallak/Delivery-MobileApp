using System;
using DeliveryApp.Domain.Enums.Order;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.OrderErrors;
using DeliveryApp.Domain.Entities.Customers.Order;

namespace DeliveryApp.Domain.Entities.Orders
{
    public class Order
    {
        public OrderID ID { get; private set; }
        public PublicCode? PublicID { get; private set; }

        public OrderType OrderType { get; private set; }

        public UserID CustomerID { get; private set; }
        public MerchantID? MerchantID { get; private set; }

        public GeoPoint PickupLocation { get; private set; } = null!;
        public GeoPoint DropoffLocation { get; private set; } = null!;

        public decimal DistanceKmSnapshot { get; private set; }
        public decimal ItemsTotalSnapshot { get; private set; }
        public decimal DeliveryFeeSnapshot { get; private set; }
        public decimal TipAmountSnapshot { get; private set; }
        public decimal TotalAmountSnapshot { get; private set; }

        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }

        public OrderStatus Status { get; private set; }

        public int RequiredDriversCount { get; private set; }

        public OrderIssueReason IssueReason { get; private set; }
        public string? IssueNote { get; private set; }

        public CancelledByType? CancelledByType { get; private set; }
        public UserID? CancelledById { get; private set; }

        public DateTimeOffset? CancelledAt { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? ConfirmedAt { get; private set; }
        public DateTimeOffset? DeliveredAt { get; private set; }

        private readonly List<OrderItem> items = new();
        public IReadOnlyCollection<OrderItem> Items => items.AsReadOnly();

        private Order() { }

        public Order(OrderID id, OrderType orderType, UserID CustomerId, MerchantID? MerchantId, decimal pickupLat,
            decimal pickupLng, decimal dropoffLat, decimal dropoffLng, decimal distanceKmSnapshot, decimal itemsTotalSnapshot, decimal deliveryFeeSnapshot,
            decimal tipAmountSnapshot, decimal totalAmountSnapshot, PaymentMethod paymentMethod, PaymentStatus paymentStatus,
            int requiredDriversCount, IEnumerable<OrderItem> orderItems, DateTimeOffset CreatedAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (CustomerId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(CustomerId));

            if (CreatedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(CreatedAtUtc));

            ValidateOrderType(orderType);
            ValidatePaymentMethod(paymentMethod);
            ValidatePaymentStatus(paymentStatus);
            ValidateMerchant(orderType, MerchantId);
            ValidateRequiredDriversCount(requiredDriversCount);

            ID = id;
            OrderType = orderType;
            CustomerID = CustomerId;
            MerchantID = MerchantId;

            SetPickupLocation(pickupLat, pickupLng);
            SetDropoffLocation(dropoffLat, dropoffLng);
            SetSnapshots(distanceKmSnapshot, itemsTotalSnapshot, deliveryFeeSnapshot, tipAmountSnapshot, totalAmountSnapshot);

            PaymentMethod = paymentMethod;
            PaymentStatus = paymentStatus;
            RequiredDriversCount = requiredDriversCount;

            SetItems(orderItems);

            IssueReason = OrderIssueReason.None;
            IssueNote = null;

            Status = OrderStatus.Draft;
            CreatedAt = CreatedAtUtc;
        }

        // ---------- Public ID ----------
        public void AssignPublicID(PublicCode publicId)
        {
            if (PublicID is not null) throw new DomainConflictException
                    (OrderErrors.PublicIdAlreadyAssignedCode, OrderErrors.PublicIdAlreadyAssignedMessage);

            PublicID = publicId;
        }

        // -------------------------
        //          Workflow
        // -------------------------

        public void StartSearchingDrivers()
        {
            CheckStatus(OrderStatus.Draft);
            Status = OrderStatus.SearchingDrivers;
        }

        public void ConfirmDrivers()
        {
            CheckStatus(OrderStatus.SearchingDrivers);
            Status = OrderStatus.DriversConfirmed;
        }

        public void MoveToAwaitingMerchantApproval()
        {
            CheckStatus(OrderStatus.DriversConfirmed);
            Status = OrderStatus.AwaitingMerchantApproval;
        }

        public void ApproveByMerchant(DateTimeOffset ConfirmedAtUtc)
        {
            CheckStatus(OrderStatus.AwaitingMerchantApproval);

            if (ConfirmedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(ConfirmedAtUtc));

            Status = OrderStatus.Preparing;
            ConfirmedAt = ConfirmedAtUtc;
        }

        public void MarkReadyForPickup()
        {
            CheckStatus(OrderStatus.Preparing);
            Status = OrderStatus.ReadyForPickup;
        }

        public void MarkPickedUp()
        {
            CheckStatus(OrderStatus.ReadyForPickup);
            Status = OrderStatus.PickedUp;
        }

        public void MarkOnTheWay()
        {
            CheckStatus(OrderStatus.PickedUp);
            Status = OrderStatus.OnTheWay;
        }

        public void MarkDelivered(DateTimeOffset deliveredAtUtc)
        {
            CheckStatus(OrderStatus.OnTheWay);

            if (deliveredAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(deliveredAtUtc));

            Status = OrderStatus.Delivered;
            DeliveredAt = deliveredAtUtc;
        }

        // -------------------------
        //    Rejection / Failure
        // -------------------------

        public void RejectByMerchant(OrderIssueReason issueReason, string? issueNote)
        {
            CheckRejectableByMerchant();
            SetIssue(issueReason, issueNote);
            Status = OrderStatus.DeliveryFailed;
        }

        public void RejectByAdmin(OrderIssueReason issueReason, string? issueNote)
        {
            CheckRejectableByAdmin();
            SetIssue(issueReason, issueNote);
            Status = OrderStatus.DeliveryFailed;
        }

        // -------------------------
        //       Cancellation
        // -------------------------

        public void Cancel(CancelledByType cancelledByType, UserID cancelledById, OrderIssueReason issueReason, string? issueNote, DateTimeOffset cancelledAtUtc)
        {
            CheckCancellable();
            ValidateCancelledByType(cancelledByType);

            if (cancelledById.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(cancelledById));

            if (cancelledAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(cancelledAtUtc));

            SetIssue(issueReason, issueNote);

            CancelledByType = cancelledByType;
            CancelledById = cancelledById;
            CancelledAt = cancelledAtUtc;
            Status = OrderStatus.Cancelled;
        }

        // -------------------------
        //         Payment
        // -------------------------

        public void MarkAsPaid()
        {
            if (PaymentStatus == PaymentStatus.Paid) return;

            PaymentStatus = PaymentStatus.Paid;
        }

        public void MarkAsUnpaid()
        {
            if (PaymentStatus == PaymentStatus.UnPaid) return;

            PaymentStatus = PaymentStatus.UnPaid;
        }

        // ---------- Helpers ----------
        public bool IsTerminal() => Status == OrderStatus.Cancelled || Status == OrderStatus.DeliveryFailed || Status == OrderStatus.Delivered;

        // -------------------------
        //        Validation
        // -------------------------

        private void CheckStatus(OrderStatus expectedStatus)
        {
            if (Status != expectedStatus) throw new DomainRuleViolationException
                    (OrderErrors.InvalidStatusTransitionCode, OrderErrors.InvalidStatusTransitionMessage);
        }

        private void CheckRejectableByMerchant()
        {
            if (Status != OrderStatus.Draft && Status != OrderStatus.SearchingDrivers && Status != OrderStatus.DriversConfirmed
                && Status != OrderStatus.AwaitingMerchantApproval && Status != OrderStatus.Preparing && Status != OrderStatus.ReadyForPickup)
                throw new DomainRuleViolationException
                    (OrderErrors.MerchantCantCancelAfterPickupCode, OrderErrors.MerchantCantCancelAfterPickupMessage);
        }

        private void CheckRejectableByAdmin()
        {
            if (IsTerminal()) throw new DomainRuleViolationException
                    (OrderErrors.CantModifyTerminalOrderCode, OrderErrors.CantModifyTerminalOrderMessage);
        }

        private void CheckCancellable()
        {
            if (Status != OrderStatus.Draft && Status != OrderStatus.SearchingDrivers && Status != OrderStatus.DriversConfirmed
                && Status != OrderStatus.AwaitingMerchantApproval) throw new DomainRuleViolationException
                    (OrderErrors.OrderCantBeCancelledAtThisStageCode, OrderErrors.OrderCantBeCancelledAtThisStageMessage);
        }

        private static void ValidateOrderType(OrderType orderType)
        {
            if (!Enum.IsDefined(typeof(OrderType), orderType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(orderType));
        }

        private static void ValidatePaymentMethod(PaymentMethod paymentMethod)
        {
            if (!Enum.IsDefined(typeof(PaymentMethod), paymentMethod)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(paymentMethod));
        }

        private static void ValidatePaymentStatus(PaymentStatus paymentStatus)
        {
            if (!Enum.IsDefined(typeof(PaymentStatus), paymentStatus)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(paymentStatus));
        }

        private static void ValidateCancelledByType(CancelledByType cancelledByType)
        {
            if (!Enum.IsDefined(typeof(CancelledByType), cancelledByType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(cancelledByType));
        }

        private static void ValidateMerchant(OrderType orderType, MerchantID? merchantId)
        {
            if (orderType == OrderType.Shipping) return;

            if (!merchantId.HasValue || merchantId.Value.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(merchantId));
        }

        private static void ValidateRequiredDriversCount(int value)
        {
            if (value < 1) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(value));
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetPickupLocation(decimal lat, decimal lng) => PickupLocation = GeoPoint.Create(lat, lng);

        private void SetDropoffLocation(decimal lat, decimal lng) => DropoffLocation = GeoPoint.Create(lat, lng);

        private void SetSnapshots(decimal distanceKm, decimal itemsTotal, decimal deliveryFee, decimal tipAmount, decimal totalAmount)
        {
            if (distanceKm < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(DistanceKmSnapshot));

            if (itemsTotal < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(ItemsTotalSnapshot));

            if (deliveryFee < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(DeliveryFeeSnapshot));

            if (tipAmount < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(TipAmountSnapshot));

            if (totalAmount < 0) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(TotalAmountSnapshot));

            if (totalAmount != itemsTotal + deliveryFee + tipAmount) throw new DomainValidationException
                    (OrderErrors.InvalidTotalAmountCode, OrderErrors.InvalidTotalAmountMessage, nameof(TotalAmountSnapshot));

            DistanceKmSnapshot = distanceKm;
            ItemsTotalSnapshot = itemsTotal;
            DeliveryFeeSnapshot = deliveryFee;
            TipAmountSnapshot = tipAmount;
            TotalAmountSnapshot = totalAmount;
        }

        private void SetItems(IEnumerable<OrderItem> orderItems)
        {
            if (orderItems is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(orderItems));

            var itemsList = orderItems.ToList();

            if (itemsList.Count == 0) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(orderItems));

            items.Clear();
            items.AddRange(itemsList);
        }

        private void SetIssue(OrderIssueReason issueReason, string? issueNote)
        {
            if (!Enum.IsDefined(typeof(OrderIssueReason), issueReason)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(issueReason));

            if (issueReason == OrderIssueReason.None) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(issueReason));

            IssueReason = issueReason;

            issueNote = NormalizeOptional(issueNote);

            if (issueNote is not null && issueNote.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(IssueNote));

            IssueNote = issueNote;
        }

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}