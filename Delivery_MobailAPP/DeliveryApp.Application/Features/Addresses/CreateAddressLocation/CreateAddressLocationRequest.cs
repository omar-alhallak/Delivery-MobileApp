namespace DeliveryApp.Application.Features.Addresses.CreateAddressLocation
{
    public sealed class CreateAddressLocationRequest // DTO لإنشاء عنوان مؤقت من الخريطة
    {
        public double Latitude { get; init; } // خط العرض المختار من الخريطة
        public double Longitude { get; init; } // خط الطول المختار من الخريطة
    }
}