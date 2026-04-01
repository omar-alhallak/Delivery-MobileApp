namespace DeliveryApp.Domain.DomainErrors.OrderErrors
{
    public static class OrderErrors
    {
        // تم تعيين معرف مسبقاً
        public const string PublicIdAlreadyAssignedCode = "Order.Public_ID_Already_Assigned";
        public const string PublicIdAlreadyAssignedMessage = "Public ID is already assigned.";

        // تجاوز حالة
        public const string InvalidStatusTransitionCode = "Order.Invalid_Status_Transition";
        public const string InvalidStatusTransitionMessage = "Order status transition is invalid.";

        // لا يمكن للمحل إلغاء الطلب بعد ما السائق يستلمه
        public const string MerchantCantCancelAfterPickupCode = "Order.Merchant_Cant_Cancel_After_Pickup";
        public const string MerchantCantCancelAfterPickupMessage = "Merchant cant cancel the order after pickup.";

        // لا يمكن تعديل الطلب عند حالات معينة
        public const string CantModifyTerminalOrderCode = "Order.Cant_Modify_Terminal_Order";
        public const string CantModifyTerminalOrderMessage = "Terminal order cant be modified.";

        // لا يمكن إلغاء الطلب في هذه الحالة 
        public const string OrderCantBeCancelledAtThisStageCode = "Order.Cant_Be_Cancelled_At_This_Stage";
        public const string OrderCantBeCancelledAtThisStageMessage = "Order cannot be cancelled at this stage.";

        // المبلغ غير صالح
        public const string InvalidTotalAmountCode = "Order.Invalid_Total_Amount";
        public const string InvalidTotalAmountMessage = "Total amount snapshot is invalid.";
    }
}