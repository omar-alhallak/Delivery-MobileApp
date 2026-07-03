namespace DeliveryApp.Application.Features.Orders.MerchantDecision
{
    public static class MerchantDecisionValidator // Validation الخاص بقرار المطعم
    {
        public static MerchantDecisionRequest ValidateReject(MerchantDecisionRequest request) // فحص بيانات الرفض
        {
            if (request is null)
                throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException("Order.Reject_Request_Required", "Reject request is required.", "request");

            var reasonValue = (int)request.IssueReason;
            if (!Enum.IsDefined(request.IssueReason) || reasonValue is < 1 or > 9)
                throw new DeliveryApp.Domain.DomainExceptions.DomainValidationException(
                    "Order.Invalid_Merchant_Reason",
                    "Merchant rejection reason is invalid.",
                    nameof(request.IssueReason));

            return request;
        }
    }
}
