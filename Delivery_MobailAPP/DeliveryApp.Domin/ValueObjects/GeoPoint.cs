using System;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Domain.ValueObjects
{
    public sealed class GeoPoint : IEquatable<GeoPoint>
    {
        private const int Scale = 6;

        public decimal Latitude { get; }
        public decimal Longitude { get; }

        private GeoPoint(decimal latitude, decimal longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static GeoPoint Create(decimal latitude, decimal longitude)
        {
            if (latitude < -90 || latitude > 90) throw new DomainValidationException
                    (ValidationErrors.InvalidLatCode, ValidationErrors.InvalidLatMessage, nameof(Latitude));

            if (longitude < -180 || longitude > 180) throw new DomainValidationException
                    (ValidationErrors.InvalidLngCode, ValidationErrors.InvalidLngMessage, nameof(Longitude));

            latitude = decimal.Round(latitude, Scale);
            longitude = decimal.Round(longitude, Scale);

            return new GeoPoint(latitude, longitude);
        }

        public override string ToString() => $"{Latitude},{Longitude}";

        public bool Equals(GeoPoint? other) => other is not null && Latitude == other.Latitude && Longitude == other.Longitude;

        public override bool Equals(object? obj) => Equals(obj as GeoPoint);

        public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
    }
}