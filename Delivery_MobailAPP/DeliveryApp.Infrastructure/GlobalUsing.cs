// -------------------------
//          Customers
// -------------------------

global using AddressID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.AddressTag>;

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
//        Engagements
// -------------------------

global using RatingID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.RatingTag>;
global using NotificationID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.NotificationTag>;

// -------------------------
//          Identity
// -------------------------

global using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;
global using UserSessionID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserSessionTag>;
global using UserIdentityID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserIdentityTag>;

// -------------------------
//          Merchants
// -------------------------

global using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;
global using MerchantWorkingHourID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantWorkingHourTag>;

// -------------------------
//          Catalog
// -------------------------

global using SystemCategoryID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.SystemCategoryTag>;

global using MerchantCategoryID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantCategoryTag>;
global using ProductID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.ProductTag>;
global using VariantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.VariantTag>;

// -------------------------
//        Moderation
// -------------------------

global using ComplaintID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.ComplaintTag>;
global using AccountWarningID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.AccountWarningTag>;

// -------------------------
//          Zones
// -------------------------

global using ZoneID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.ZoneTag>;