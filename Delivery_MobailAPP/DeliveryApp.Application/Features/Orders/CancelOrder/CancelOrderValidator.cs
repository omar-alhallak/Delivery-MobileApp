using DeliveryApp.Domain.Enums.OrderEnums;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Application.Features.Orders.CancelOrder
{
    public static class CancelOrderValidator // Validation الخاص بإلغاء الطلب
    {
        public static CancelOrderRequest Validate(CancelOrderRequest request) // التأكد أن بيانات الإلغاء موجودة
        {
            if (request is null)
                throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException("Order.Cancel_Request_Required", "Cancel request is required.", "request");

            return request;
        }

        public static void ValidateReason(CancelOrderRequest request, DeliveryApp.Domain.Enums.OrderEnums.CancelledByType actor)
        {
            var reasonValue = (int)request.IssueReason;
            var isDefined = Enum.IsDefined(request.IssueReason);
            var isAllowed = actor switch
            {
                CancelledByType.Customer => reasonValue is >= 20 and <= 29,
                CancelledByType.Merchant => reasonValue is >= 1 and <= 9,
                _ => false
            };

            if (!isDefined || !isAllowed)
                throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException(
                    "Order.Invalid_Cancellation_Reason",
                    "Cancellation reason is not valid for the current user.",
                    nameof(request.IssueReason));

            if (actor == CancelledByType.Merchant &&
                      request.IssueReason == OrderIssueReason.VendorRejectedOther &&
                      string.IsNullOrWhiteSpace(request.IssueNote))
            {
                throw new DomainValidationException(
                    "Order.Cancellation_Note_Required",
                    "Cancellation note is required when the reason is Other.",
                    nameof(request.IssueNote));
            }
        }
    }
}
