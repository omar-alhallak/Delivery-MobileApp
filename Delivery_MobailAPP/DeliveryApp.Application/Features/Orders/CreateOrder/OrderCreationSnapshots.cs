namespace DeliveryApp.Application.Features.Orders.CreateOrder
{
    public sealed record OrderCreationLocationSnapshot(
        double PickupLatitude,
        double PickupLongitude,
        double DropoffLatitude,
        double DropoffLongitude);

    public sealed record OrderCatalogItemSnapshot(
        string ProductName,
        string? VariantName,
        decimal UnitPrice);
}
