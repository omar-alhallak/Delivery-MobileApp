using DeliveryApp.Domain.Enums.CustomerEnums;

namespace DeliveryApp.Application.Features.Addresses.UpdateAddressDetails
{
    public sealed class UpdateAddressDetailsRequest // DTO لتعديل تفاصيل العنوان بعد إكماله
    {
        public string Label { get; init; } = null!; // اسم العنوان
        public AddressType AddressType { get; init; } // نوع العنوان
        public string BuildingName { get; init; } = null!; // اسم أو رقم البناء
        public string Floor { get; init; } = null!; // الطابق
        public string DoorInfo { get; init; } = null!; // معلومات الباب أو الشقة
        public string? Notes { get; init; } // ملاحظات اختيارية
    }
}