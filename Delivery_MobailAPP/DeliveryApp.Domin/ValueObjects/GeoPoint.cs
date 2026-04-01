using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.ValueObjects
{
    public sealed class GeoPoint : IEquatable<GeoPoint> // يفرض قواعد الأحداثيات ويجمع نقطتان في Point
    {                                                   // ما يعني Point = Lat ,Lng
        private const int Scale = 6; // الأرقام بعد الفاصلة

        public decimal Latitude { get; }
        public decimal Longitude { get; }

        private GeoPoint(decimal latitude, decimal longitude) // لمنع إنشاء الصف إلا عن طريق Create
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static GeoPoint Create(decimal latitude, decimal longitude) // تتحقق و تنظف و تنشأ النقاط
        {
            if (latitude < -90 || latitude > 90) throw new DomainValidationException
                    (ValidationErrors.InvalidLatCode, ValidationErrors.InvalidLatMessage, nameof(Latitude));

            if (longitude < -180 || longitude > 180) throw new DomainValidationException
                    (ValidationErrors.InvalidLngCode, ValidationErrors.InvalidLngMessage, nameof(Longitude));

            latitude = decimal.Round(latitude, Scale);
            longitude = decimal.Round(longitude, Scale);

            return new GeoPoint(latitude, longitude);
        }

        public override string ToString() => $"{Latitude},{Longitude}"; // لتحويل الكلاس لنص

        // لمقارنة القيم المدخلة من دونه لا يمكن المقارنة لان كل قيمة تعتبر مختلفة بذاكرة
        // الأولى لمقارنة نقطة مع نقطة
        public bool Equals(GeoPoint? other) => other is not null && Latitude == other.Latitude && Longitude == other.Longitude;

        // الثانية لمقارنة أي نوع ثم تحوله ل نقطة
        public override bool Equals(object? obj) => Equals(obj as GeoPoint);

        // تعطي رقم يمثل القيمة للمقارنة
        public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
    }
}