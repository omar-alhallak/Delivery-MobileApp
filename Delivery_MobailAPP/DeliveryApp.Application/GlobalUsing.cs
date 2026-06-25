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
//         Engagements
// -------------------------

global using RatingID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.RatingTag>;
global using NotificationID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.NotificationTag>;
