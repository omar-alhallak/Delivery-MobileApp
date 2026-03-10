using System;

namespace DeliveryApp.Domain.DomainErrors.OrderErrors
{
    public static class OrderErrors
    {
        public const string PublicIdAlreadyAssignedCode = "Order.Public_Id_Already_Assigned";
        public const string PublicIdAlreadyAssignedMessage = "Public ID is already assigned.";

        public const string InvalidStatusTransitionCode = "Order.Invalid_Status_Transition";
        public const string InvalidStatusTransitionMessage = "Order status transition is invalid.";

        public const string MerchantCantCancelAfterPickupCode = "Order.Merchant_Cannot_Cancel_After_Pickup";
        public const string MerchantCantCancelAfterPickupMessage = "Merchant cannot cancel the order after pickup.";

        public const string CantModifyTerminalOrderCode = "Order.Cannot_Modify_Terminal_Order";
        public const string CantModifyTerminalOrderMessage = "Terminal order cannot be modified.";

        public const string OrderCantBeCancelledAtThisStageCode = "Order.Cannot_Be_Cancelled_At_This_Stage";
        public const string OrderCantBeCancelledAtThisStageMessage = "Order cannot be cancelled at this stage.";

        public const string InvalidTotalAmountCode = "Order.Invalid_Total_Amount";
        public const string InvalidTotalAmountMessage = "Total amount snapshot is invalid.";
    }
}