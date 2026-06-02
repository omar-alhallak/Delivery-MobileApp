namespace DeliveryApp.Application.Features.Orders.MerchantDecision
{
    public static class MerchantDecisionValidator // Validation الخاص بقرار المطعم
    {
        public static MerchantDecisionRequest ValidateReject(MerchantDecisionRequest request) // فحص بيانات الرفض
        {
            if (request is null) throw new Exception("Reject request is required.");
            return request;
        }
    }
}