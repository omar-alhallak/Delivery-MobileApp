using DeliveryApp.Domain.Enums.OrderEnums;

namespace DeliveryApp.Application.Features.Orders.CancelOrder
{
    public sealed class CancelOrderRequest
    {
        public CancelledByType CancelledByType { get; init; } 
        public Guid CancelledById { get; init; } 
        public OrderIssueReason IssueReason { get; init; }
        public string? IssueNote { get; init; }
    }
}