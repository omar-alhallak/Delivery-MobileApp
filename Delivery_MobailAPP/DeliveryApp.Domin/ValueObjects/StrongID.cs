using System;

namespace DeliveryApp.Domain.ValueObjects
{
    public readonly record struct StrongID<TTag>(Guid Value) // تنظيم إنشاء واستدعاء ID
    {                                                        // منع خلط الأيديات مع بعضها
        public static StrongID<TTag> New() => new(Guid.NewGuid());

        public static StrongID<TTag> From(Guid value) => value == Guid.Empty
                ? throw new ArgumentException("ID Cannot be empty.", nameof(value)) : new StrongID<TTag>(value);

        public bool IsEmpty => Value == Guid.Empty;

        public override string ToString() => Value.ToString("D");
    }

    // ================== Tags ==================

    public readonly struct UserTag { }
    public readonly struct UserIdentityTag { }
    public readonly struct UserSessionTag { }

    public readonly struct AddressTag { }

    public readonly struct MerchantTag { }

    public readonly struct SystemCategoryTag { }
    public readonly struct MerchantCategoryTag { }
    public readonly struct ProductTag { }
    public readonly struct VariantTag { }

    public readonly struct OrderTag { }
    public readonly struct OrderItemTag { }

    public readonly struct VehicleTypeTag { }
    public readonly struct DriverRequestTag { }
    public readonly struct ZoneTag { }
    public readonly struct AccountWarningTag { }
    public readonly struct ComplaintTag { }

}