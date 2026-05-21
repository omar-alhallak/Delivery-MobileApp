using DeliveryApp.Domain.Entities.Customers.Orders;

namespace DeliveryApp.Application.Features.Orders.Common
{
    public static class OrderMapper // يحول كائنات الدومين إلى DTOs مفهومة للـ API
    {
        public static OrderDto ToDto(Order order, IEnumerable<OrderAssignment> assignments) // تحويل الطلب الكامل مع المنتجات والتعيينات
        {
            return new OrderDto
            {
                Id = order.ID.Value,
                PublicId = order.PublicID?.Value,
                OrderType = order.OrderType,
                CustomerId = order.CustomerID.Value,
                MerchantId = order.MerchantID?.Value,
                PickupLatitude = order.PickupLocation.Latitude,
                PickupLongitude = order.PickupLocation.Longitude,
                DropoffLatitude = order.DropoffLocation.Latitude,
                DropoffLongitude = order.DropoffLocation.Longitude,
                DistanceKm = order.DistanceKmSnapshot,
                ItemsTotal = order.ItemsTotalSnapshot,
                DeliveryFee = order.DeliveryFeeSnapshot,
                TipAmount = order.TipAmountSnapshot,
                TotalAmount = order.TotalAmountSnapshot,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                Status = order.Status,
                RequiredDriversCount = order.RequiredDriversCount,
                IssueReason = order.IssueReason,
                IssueNote = order.IssueNote,
                CancelledByType = order.CancelledByType,
                CancelledById = order.CancelledById?.Value,
                CancelledAt = order.CancelledAt,
                CreatedAt = order.CreatedAt,
                ConfirmedAt = order.ConfirmedAt,
                DeliveredAt = order.DeliveredAt,
                Items = order.Items.Select(ToDto).ToList(),
                Assignments = assignments.Select(ToDto).ToList()
            };
        }

        private static OrderItemDto ToDto(OrderItem item) // تحويل منتج واحد من الدومين إلى DTO
        {
            return new OrderItemDto
            {
                Id = item.ID.Value,
                ProductName = item.ProductNameSnapshot,
                VariantName = item.VariantNameSnapshot,
                UnitPrice = item.UnitPriceSnapshot,
                Quantity = item.Quantity,
                LineTotal = item.LineTotalSnapshot,
                CustomerNote = item.CustomerNote
            };
        }

        private static OrderAssignmentDto ToDto(OrderAssignment assignment) // تحويل تعيين السائق إلى DTO
        {
            return new OrderAssignmentDto
            {
                Id = assignment.ID,
                OrderId = assignment.OrderID.Value,
                DriverId = assignment.DriverID.Value,
                AssignedAt = assignment.AssignedAt,
                Status = assignment.Status,
                RemovedAt = assignment.RemovedAt,
                RemoveReason = assignment.RemoveReason
            };
        }
    }
}
