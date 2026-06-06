using DeliveryApp.Domain.Enums.CustomerEnums;
using DeliveryApp.Domain.ValueObjects;

namespace DeliveryApp.Application.Features.Addresses.CreateAddress
{
    public static class CreateAddressValidator
    {
        public static CreateUserAddressValidatedInput Validate(UserID userId, CreateUserAddressRequest request)
        {
            if (request is null)
                throw new Exception("بيانات الطلب مطلوبة.");

            // فحص وتعقيم الحقول حقل بحقل مع رسائل عربية
            var label = ValidateLabel(request.Label);
            var addressType = ValidateAddressType(request.AddressType);
            var location = ValidateLocation(request.Location);
            var buildingName = ValidateBuildingName(request.BuildingName);

            // تمرير أسماء الحقول بالعربي للميثود المساعدة
            var floor = NormalizeOptionalString(request.Floor, 50, "الطابق");
            var doorInfo = NormalizeOptionalString(request.DoorInfo, 50, "معلومات الشقة/الباب");
            var notes = NormalizeOptionalString(request.Notes, 500, "الملاحظات");

            // إرجاع الكائن النظيف والمحمي بالكامل
            return new CreateUserAddressValidatedInput
            (
                userId,
                label,
                addressType,
                location,
                buildingName,
                floor,
                doorInfo,
                notes,
                request.IsDefault,
                request.IsTemporary
            );
        }

        // -------------------------
        // Label Validation
        // -------------------------
        private static string ValidateLabel(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("تسمية العنوان مطلوبة (مثل: المنزل، العمل).");

            value = value.Trim();

            if (value.Length > 50)
                throw new Exception("تسمية العنوان طويلة جداً. الحد الأقصى 50 حرفاً.");

            return value;
        }

        // -------------------------
        // AddressType Validation
        // -------------------------
        private static AddressType ValidateAddressType(AddressType? value)
        {
            if (value is null)
                throw new Exception("نوع العنوان مطلوب.");

            if (!Enum.IsDefined(typeof(AddressType), value.Value))
                throw new Exception("نوع العنوان المحدد غير صالح.");

            return value.Value;
        }

        // -------------------------
        // GeoPoint (Location) Validation
        // -------------------------
        private static GeoPoint ValidateLocation(GeoPoint? value)
        {
            if (value is null)
                throw new Exception("إحداثيات الموقع (GPS) مطلوبة.");

            if (value.Latitude < -90 || value.Latitude > 90)
                throw new Exception("موقع غير صالح: خط العرض يجب أن يكون بين -90 و 90 درجة.");

            if (value.Longitude < -180 || value.Longitude > 180)
                throw new Exception("موقع غير صالح: خط الطول يجب أن يكون بين -180 و 180 درجة.");

            return value;
        }

        // -------------------------
        // Building Name Validation
        // -------------------------
        private static string ValidateBuildingName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("اسم أو رقم البناية مطلوب.");

            value = value.Trim();

            if (value.Length > 150)
                throw new Exception("اسم البناية طويل جداً. الحد الأقصى 150 حرفاً.");

            return value;
        }

        // -------------------------
        // Helper for Optional Strings
        // -------------------------
        private static string? NormalizeOptionalString(string? value, int maxLength, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();

            if (value.Length > maxLength)
                throw new Exception($"حقل {fieldName} طويل جداً. الحد الأقصى {maxLength} حرفاً.");

            return value;
        }
    }

    // الـ Record النظيف والمحمي
    public sealed record CreateUserAddressValidatedInput
    (
        UserID UserID,
        string Label,
        AddressType AddressType,
        GeoPoint Location,
        string BuildingName,
        string? Floor,
        string? DoorInfo,
        string? Notes,
        bool IsDefault,
        bool IsTemporary
    );
}

