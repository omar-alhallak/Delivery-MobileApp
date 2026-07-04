using DeliveryApp.Domain.Enums.OrderEnums;

namespace DeliveryApp.Application.Features.Orders.CancelOrder
{
    public sealed class CancelOrderRequest
    {
        public OrderIssueReason IssueReason { get; init; }
        public string? IssueNote { get; init; }
    }
}
