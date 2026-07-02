using DeliveryApp.Domain.Enums.OrderEnums;

namespace DeliveryApp.Application.Features.Orders.MerchantDecision
{
    public sealed class MerchantDecisionRequest
    {
        public OrderIssueReason IssueReason { get; init; }
        public string? IssueNote { get; init; }
    }
}