using DeliveryApp.Domain.Enums.CustomerEnums;

namespace DeliveryApp.Application.Features.Addresses.CompleteAddress
{
    public sealed class CompleteAddressRequest // DTO لإكمال تفاصيل العنوان المؤقت
    {
        public string Label { get; init; } = null!; // اسم العنوان
        public AddressType AddressType { get; init; } // نوع العنوان
        public string BuildingName { get; init; } = null!; // اسم أو رقم البناء
        public string Floor { get; init; } = null!; // الطابق
        public string DoorInfo { get; init; } = null!; // معلومات الباب أو الشقة
        public string? Notes { get; init; } // ملاحظات اختيارية
    }
}
