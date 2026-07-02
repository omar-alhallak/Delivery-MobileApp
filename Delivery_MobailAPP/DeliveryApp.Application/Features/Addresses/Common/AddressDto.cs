using DeliveryApp.Domain.Enums.CustomerEnums;

namespace DeliveryApp.Application.Features.Addresses.Common
{
    public sealed class AddressDto // DTO يعرض بيانات العنوان للواجهة
    {
        public Guid Id { get; init; } // معرف العنوان
        public Guid UserId { get; init; } // صاحب العنوان
        public string? Label { get; init; } // اسم مختصر مثل البيت أو العمل
        public AddressType? AddressType { get; init; } // نوع العنوان
        public double Latitude { get; init; } // خط العرض
        public double Longitude { get; init; } // خط الطول
        public string? BuildingName { get; init; } // اسم أو رقم البناء
        public string? Floor { get; init; } // الطابق
        public string? DoorInfo { get; init; } // معلومات الباب أو الشقة
        public string? Notes { get; init; } // ملاحظات إضافية
        public bool IsDefault { get; init; } // هل هو العنوان الافتراضي
        public bool IsTemporary { get; init; } // هل ما زال يحتاج تفاصيل
        public bool IsActive { get; init; } // هل العنوان فعال
        public DateTimeOffset CreatedAt { get; init; } // وقت الإنشاء
    }
}
