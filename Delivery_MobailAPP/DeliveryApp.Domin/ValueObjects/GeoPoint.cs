using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.ValueObjects
{
    public sealed class GeoPoint : IEquatable<GeoPoint> // يفرض قواعد الأحداثيات ويجمع نقطتان في Point
    {                                                   // أنو يعني Point = Lat ,Lng
        private const int Scale = 6; // الأرقام بعد الفاصلة
        private const double Tolerance = 1e-6; // الفرق الي بميز نقطة عن نقطة
        private const double EarthRadiusKm = 6371d; // نصف قطر الأرض بالكيلومتر

        public double Latitude { get; } // إحداثيات الطول
        public double Longitude { get; } // إحداثيات العرض

        private GeoPoint(double latitude, double longitude) // لمنع إنشاء الصف إلا عن طريق Create
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static GeoPoint Create(double latitude, double longitude) // إنشاء النقاط
        {
            if (latitude < -90 || latitude > 90) throw new DomainValidationException
                    (ValidationErrors.InvalidLatCode, ValidationErrors.InvalidLatMessage, nameof(Latitude));

            if (longitude < -180 || longitude > 180) throw new DomainValidationException
                    (ValidationErrors.InvalidLngCode, ValidationErrors.InvalidLngMessage, nameof(Longitude));

            latitude = Math.Round(latitude, Scale);
            longitude = Math.Round(longitude, Scale);

            return new GeoPoint(latitude, longitude);
        }

        public double DistanceTo(GeoPoint other) // حساب المسافة التقريبية بين نقطتين بالكيلومتر
        {
            if (other is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(other));

            var lat1Rad = DegreesToRadians(Latitude);
            var lat2Rad = DegreesToRadians(other.Latitude);
            var deltaLatRad = DegreesToRadians(other.Latitude - Latitude);
            var deltaLngRad = DegreesToRadians(other.Longitude - Longitude);

            var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                    Math.Sin(deltaLngRad / 2) * Math.Sin(deltaLngRad / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return Math.Round(EarthRadiusKm * c, 3);
        }

        public override string ToString() => $"{Latitude},{Longitude}"; // لتحويل الكلاس لنص

        // لمقارنة القيم المدخلة من دونه لا يمكن المقارنة لان كل قيمة تعتبر مختلفة بذاكرة
        // الأولى لمقارنة نقطة مع نقطة
        public bool Equals(GeoPoint? other) => other is not null &&
            Math.Abs(Latitude - other.Latitude) < Tolerance &&
            Math.Abs(Longitude - other.Longitude) < Tolerance;

        // الثانية لمقارنة أي نوع ثم تحوله ل نقطة
        public override bool Equals(object? obj) => Equals(obj as GeoPoint);

        // تعطي رقم يمثل القيمة للمقارنة
        public override int GetHashCode()
        {
            var lat = Math.Round(Latitude, Scale);
            var lng = Math.Round(Longitude, Scale);

            return HashCode.Combine(lat, lng);
        }

        private static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180d); // تحويل الزاوية لدرجة
    }
}