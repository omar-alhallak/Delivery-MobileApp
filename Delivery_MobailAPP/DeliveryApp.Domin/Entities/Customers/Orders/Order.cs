using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.OrderEnums;
using DeliveryApp.Domain.DomainErrors.OrderErrors;
using DeliveryApp.Domain.Entities.Customers.Orders;

namespace DeliveryApp.Domain.Entities.Orders
{
    public class Order // يمثل الطلب
    {
        // -------------------------
        //            Key
        // -------------------------

        public OrderID ID { get; private set; } // PK معرف الطلب
        public PublicCode? PublicID { get; private set; } // الكود الي بيظهر للمستخدم

        // -------------------------
        //            Type
        // -------------------------

        public OrderType OrderType { get; private set; } // نوع الطلب

        // -------------------------
        //         Relations
        // -------------------------

        public UserID CustomerID { get; private set; } // الزبون
        public MerchantID? MerchantID { get; private set; } // المطعم الي تم التوصيل منه

        // -------------------------
        //         Locations
        // -------------------------

        public GeoPoint PickupLocation { get; private set; } = null!; // موقع الاستلام
        public GeoPoint DropoffLocation { get; private set; } = null!; // موقع التسليم

        // -------------------------
        //         Snapshots
        // -------------------------

        public double DistanceKmSnapshot { get; private set; } // المسافة تبع الطلب
        public decimal ItemsTotalSnapshot { get; private set; } // مجموع العناصر
        public decimal DeliveryFeeSnapshot { get; private set; } // رسوم التوصيل
        public decimal TipAmountSnapshot { get; private set; } // البقشيش
        public decimal TotalAmountSnapshot { get; private set; } // الإجمالي

        // -------------------------
        //          Payment
        // -------------------------

        public PaymentMethod PaymentMethod { get; private set; } // طريقة الدفع
        public PaymentStatus PaymentStatus { get; private set; } // حالة الدفع

        // -------------------------
        //          Status
        // -------------------------

        public OrderStatus Status { get; private set; } // حالة الطلب

        // -------------------------
        //          Drivers
        // -------------------------

        public int RequiredDriversCount { get; private set; } // عدد السائقين المطلوبين

        // -------------------------
        //           Issue
        // -------------------------

        public OrderIssueReason IssueReason { get; private set; } // سبب المشكلة
        public string? IssueNote { get; private set; } // ملاحظة

        // -------------------------
        //       Cancellation
        // -------------------------

        public CancelledByType? CancelledByType { get; private set; } // مين ألغى
        public UserID? CancelledById { get; private set; } // معرف الملغي

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset? CancelledAt { get; private set; } // وقت الإلغاء
        public DateTimeOffset CreatedAt { get; private set; } // وقت الإنشاء
        public DateTimeOffset? ConfirmedAt { get; private set; } // وقت الموافقة
        public DateTimeOffset? DeliveredAt { get; private set; } // وقت التسليم

        // -------------------------
        //           Items          عناصر الطلب 
        // -------------------------

        private readonly List<OrderItem> items = new();
        public IReadOnlyCollection<OrderItem> Items => items.AsReadOnly();

        private Order() { }

        public Order(OrderID id, OrderType orderType, UserID customerId, MerchantID? merchantId, double pickupLat, double pickupLng,
            double dropoffLat, double dropoffLng, double distanceKmSnapshot, decimal itemsTotalSnapshot, decimal deliveryFeeSnapshot,
            decimal tipAmountSnapshot, decimal totalAmountSnapshot, PaymentMethod paymentMethod, PaymentStatus paymentStatus,
            int requiredDriversCount, IEnumerable<OrderItem> orderItems, DateTimeOffset createdAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (customerId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(customerId));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            ValidateOrderType(orderType);
            ValidatePaymentMethod(paymentMethod);
            ValidatePaymentStatus(paymentStatus);
            ValidateMerchant(orderType, merchantId);
            ValidateRequiredDriversCount(requiredDriversCount);

            ID = id;
            OrderType = orderType;
            CustomerID = customerId;
            MerchantID = merchantId;

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
            CreatedAt = createdAtUtc;
        }

        // ---------- Public ID ----------
        public void AssignPublicID(PublicCode publicId) // تعيين الكود الي بيظهر للمستخدمين
        {
            if (PublicID is not null) throw new DomainConflictException
                    (OrderErrors.PublicIdAlreadyAssignedCode, OrderErrors.PublicIdAlreadyAssignedMessage);

            PublicID = publicId;
        }

        // -------------------------
        //          Workflow         الهدف أن يمر طلب بجميع الحالات دون تجاوز أي مرحلة
        // -------------------------

        public void StartSearchingDrivers() => ChangeStatus(OrderStatus.Draft, OrderStatus.SearchingDrivers); // بدء البحث عن سائقين

        public void ConfirmDrivers() => ChangeStatus(OrderStatus.SearchingDrivers, OrderStatus.DriversConfirmed); // تم تأكيد من قبل السائقين

        public void MoveToAwaitingMerchantApproval() => ChangeStatus(OrderStatus.DriversConfirmed, OrderStatus.AwaitingMerchantApproval); // انتظار موافقة المطعم

        public void ApproveByMerchant(DateTimeOffset confirmedAtUtc) // المطعم وافق
        {
            EnsureStatus(OrderStatus.AwaitingMerchantApproval);

            if (confirmedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(confirmedAtUtc));

            if (confirmedAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(confirmedAtUtc));

            Status = OrderStatus.Preparing;
            ConfirmedAt = confirmedAtUtc;
        }

        public void MarkReadyForPickup() => ChangeStatus(OrderStatus.Preparing, OrderStatus.ReadyForPickup); // تم تجهيز الطلب

        public void MarkPickedUp() => ChangeStatus(OrderStatus.ReadyForPickup, OrderStatus.PickedUp); // تم الاستلام

        public void MarkOnTheWay() => ChangeStatus(OrderStatus.PickedUp, OrderStatus.OnTheWay); // بالطريق

        public void MarkDelivered(DateTimeOffset deliveredAtUtc) // تم التسليم
        {
            EnsureStatus(OrderStatus.OnTheWay);

            if (deliveredAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(deliveredAtUtc));

            if (deliveredAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(deliveredAtUtc));

            Status = OrderStatus.Delivered;
            DeliveredAt = deliveredAtUtc;
        }

        // -------------------------
        //    Rejection / Failure
        // -------------------------

        public void RejectByMerchant(OrderIssueReason issueReason, string? issueNote) // رفض من المطعم
        {
            EnsureRejectableByMerchant();
            SetIssue(issueReason, issueNote);
            Status = OrderStatus.DeliveryFailed;
        }

        public void RejectByAdmin(OrderIssueReason issueReason, string? issueNote) // رفض من المشرف
        {
            EnsureRejectableByAdmin();
            SetIssue(issueReason, issueNote);
            Status = OrderStatus.DeliveryFailed;
        }

        // -------------------------
        //       Cancellation
        // -------------------------

        public void Cancel(CancelledByType cancelledByType, UserID cancelledById, OrderIssueReason issueReason, string? issueNote, DateTimeOffset cancelledAtUtc) // إلغاء الطلب
        {
            EnsureCancellable();
            ValidateCancelledByType(cancelledByType);

            if (cancelledById.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(cancelledById));

            if (cancelledAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(cancelledAtUtc));

            if (cancelledAtUtc < CreatedAt) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(cancelledAtUtc));

            SetIssue(issueReason, issueNote);

            CancelledByType = cancelledByType;
            CancelledById = cancelledById;
            CancelledAt = cancelledAtUtc;
            Status = OrderStatus.Cancelled;
        }

        // -------------------------
        //         Payment
        // -------------------------

        public void MarkAsPaid() // تم الدفع
        {
            if (PaymentStatus == PaymentStatus.Paid) return;
            PaymentStatus = PaymentStatus.Paid;
        }

        public void MarkAsUnpaid() // غير مدفوع
        {
            if (PaymentStatus == PaymentStatus.UnPaid) return;
            PaymentStatus = PaymentStatus.UnPaid;
        }

        // -------------------------
        //         Helpers
        // -------------------------

        public bool IsFinal() => Status == OrderStatus.Cancelled || Status == OrderStatus.DeliveryFailed || Status == OrderStatus.Delivered; // تفحص أن طلب وصل لاخر مرحلة 

        private void ChangeStatus(OrderStatus from, OrderStatus to) // تغيير من حالة لحالة
        {
            if (Status != from) throw new DomainRuleViolationException
                    (OrderErrors.InvalidStatusTransitionCode, OrderErrors.InvalidStatusTransitionMessage);

            Status = to;
        }

        // -------------------------
        //        Validation
        // -------------------------

        private void EnsureStatus(OrderStatus expectedStatus) // تحقق من صحة الحالة قبل تنفيذها
        {
            if (Status != expectedStatus) throw new DomainRuleViolationException
                    (OrderErrors.InvalidStatusTransitionCode, OrderErrors.InvalidStatusTransitionMessage);
        }

        private void EnsureRejectableByMerchant() // تحقق من أن المطعم يستطيع الرفض
        {
            if (Status != OrderStatus.Draft && Status != OrderStatus.SearchingDrivers && Status != OrderStatus.DriversConfirmed
                && Status != OrderStatus.AwaitingMerchantApproval && Status != OrderStatus.Preparing && Status != OrderStatus.ReadyForPickup)
                throw new DomainRuleViolationException(OrderErrors.MerchantCantCancelAfterPickupCode, OrderErrors.MerchantCantCancelAfterPickupMessage);
        }

        private void EnsureRejectableByAdmin() // تحقق من أن المشرف يستطيع الرفض
        {
            if (IsFinal()) throw new DomainRuleViolationException
                    (OrderErrors.CantModifyTerminalOrderCode, OrderErrors.CantModifyTerminalOrderMessage);
        }

        private void EnsureCancellable() // التحقق من أن الطلب لسا قابل للإلغاء
        {
            if (Status != OrderStatus.Draft && Status != OrderStatus.SearchingDrivers && Status != OrderStatus.DriversConfirmed
                && Status != OrderStatus.AwaitingMerchantApproval) throw new DomainRuleViolationException
                    (OrderErrors.OrderCantBeCancelledAtThisStageCode, OrderErrors.OrderCantBeCancelledAtThisStageMessage);
        }

        private static void ValidateOrderType(OrderType orderType) // تحقق من صحة نوع الطلب
        {
            if (!Enum.IsDefined(typeof(OrderType), orderType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(orderType));
        }

        private static void ValidatePaymentMethod(PaymentMethod paymentMethod) // تحقق من صحة طريقة الدفع
        {
            if (!Enum.IsDefined(typeof(PaymentMethod), paymentMethod)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(paymentMethod));
        }

        private static void ValidatePaymentStatus(PaymentStatus paymentStatus) // تحقق من صحة حالة الدفع
        {
            if (!Enum.IsDefined(typeof(PaymentStatus), paymentStatus)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(paymentStatus));
        }

        private static void ValidateCancelledByType(CancelledByType cancelledByType) // تحقق من صحة نوع الإلغاء
        {
            if (!Enum.IsDefined(typeof(CancelledByType), cancelledByType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(cancelledByType));
        }

        private static void ValidateMerchant(OrderType orderType, MerchantID? merchantId) // تحقق من وجود المطعم
        {
            if (orderType == OrderType.Shipping) return;

            if (!merchantId.HasValue || merchantId.Value.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(merchantId));
        }

        private static void ValidateRequiredDriversCount(int value) // تحقق من عدد سائقين المطلوبين
        {
            if (value < 1 || value > 5) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(value));
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetPickupLocation(double lat, double lng) => PickupLocation = GeoPoint.Create(lat, lng); // إدخال موقع الإستلام

        private void SetDropoffLocation(double lat, double lng) => DropoffLocation = GeoPoint.Create(lat, lng); // إدخال موقع التسليم

        private void SetSnapshots(double distanceKm, decimal itemsTotal, decimal deliveryFee, decimal tipAmount, decimal totalAmount) // إدخال القيم المالية
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

        private void SetItems(IEnumerable<OrderItem> orderItems) // إدخال عناصر الطلب
        {
            if (orderItems is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(orderItems));

            var itemsList = orderItems.ToList();

            if (itemsList.Count == 0) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(orderItems));

            if (itemsList.Any(x => x is null)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(orderItems));

            items.Clear();
            items.AddRange(itemsList);
        }

        private void SetIssue(OrderIssueReason issueReason, string? issueNote) // إدخال سبب المشكلة
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

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim(); // تنظيف النص
    }
}