// -------------------------
//          Identity
// -------------------------

global using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;
global using UserIdentityID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserIdentityTag>;
global using UserSessionID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserSessionTag>;

// -------------------------
//          Customers
// -------------------------

global using AddressID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.AddressTag>;

// -------------------------
//          Merchants
// -------------------------

global using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;

// -------------------------
//          Catalog
// -------------------------

global using SystemCategoryID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.SystemCategoryTag>;
global using MerchantCategoryID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantCategoryTag>;
global using ProductID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.ProductTag>;
global using VariantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.VariantTag>;

// -------------------------
//           Orders
// -------------------------

global using OrderID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.OrderTag>;
global using OrderItemID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.OrderItemTag>;

// -------------------------
//          Drivers
// -------------------------

global using DriverLocationID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.DriverLocationTag>;
global using DriverRequestID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.DriverRequestTag>;
global using VehicleTypeID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.VehicleTypeTag>;

// -------------------------
//        Moderation
// -------------------------

global using ZoneID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.ZoneTag>;
global using AccountWarningID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.AccountWarningTag>;
global using ComplaintID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.ComplaintTag>;

// -------------------------
//        Engagements
// -------------------------

global using RatingID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.RatingTag>;
global using NotificationID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.NotificationTag>;