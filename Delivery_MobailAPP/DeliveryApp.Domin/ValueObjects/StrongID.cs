using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.ValueObjects
{
    public readonly record struct StrongID<TTag>(Guid Value) // ID مميز لكل كلاس
    {                                                        // لمنع خلط الأيديات مع بعضها
        public static StrongID<TTag> New() => new(Guid.NewGuid()); // لإنشاء ID

        //  تحويل Guid إلى StrongID
        public static StrongID<TTag> From(Guid value) => value == Guid.Empty ? throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(value)) : new StrongID<TTag>(value);

        public bool IsEmpty => Value == Guid.Empty; // تفحص أنه ماله فاضي

        public override string ToString() => Value.ToString("D"); // يرجع Guid كنص
    }
    
// ====================== Tags ======================

    // -------------------------
    //          Identity
    // -------------------------

    public readonly struct UserTag { }
    public readonly struct UserIdentityTag { }
    public readonly struct UserSessionTag { }

    // -------------------------
    //          Customers
    // -------------------------

    public readonly struct AddressTag { }

    // -------------------------
    //          Merchants
    // -------------------------

    public readonly struct MerchantTag { }

    // -------------------------
    //          Catalog
    // -------------------------

    public readonly struct SystemCategoryTag { }
    public readonly struct MerchantCategoryTag { }
    public readonly struct ProductTag { }
    public readonly struct VariantTag { }

    // -------------------------
    //           Orders
    // -------------------------

    public readonly struct OrderTag { }
    public readonly struct OrderItemTag { }

    // -------------------------
    //          Drivers
    // -------------------------

    public readonly struct DriverLocationTag { }
    public readonly struct DriverRequestTag { }
    public readonly struct VehicleTypeTag { }

    // -------------------------
    //        Moderation
    // -------------------------

    public readonly struct ZoneTag { }
    public readonly struct AccountWarningTag { }
    public readonly struct ComplaintTag { }

    // -------------------------
    //        Engagements
    // -------------------------

    public readonly struct RatingTag { }
    public readonly struct NotificationTag { }
}