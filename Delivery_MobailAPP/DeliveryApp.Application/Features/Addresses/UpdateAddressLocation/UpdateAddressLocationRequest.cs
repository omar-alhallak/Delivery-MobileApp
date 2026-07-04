namespace DeliveryApp.Application.Features.Addresses.UpdateAddressLocation
{
    public sealed class UpdateAddressLocationRequest // DTO لتعديل موقع العنوان المؤقت فقط
    {
        public double Latitude { get; init; } // خط العرض الجديد
        public double Longitude { get; init; } // خط الطول الجديد
    }
}