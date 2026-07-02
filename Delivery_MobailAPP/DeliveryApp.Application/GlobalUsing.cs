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

// -------------------------
//           Orders
// -------------------------

global using OrderID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.OrderTag>;
global using OrderItemID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.OrderItemTag>;