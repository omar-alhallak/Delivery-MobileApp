using DeliveryApp.Domain.Enums.CustomerEnums;

namespace DeliveryApp.Application.Features.Addresses.UpdateAddress
{
    public static class UpdateAddressValidator
    {
        public static UpdateAddressValidatedInput Validate(UserID userId, AddressID addressId, UpdateAddressRequest request)
        {
            if (request is null)
                throw new Exception("بيانات التعديل مطلوبة.");

            if (addressId == default)
                throw new Exception("معرّف العنوان غير صالح أو مفقود.");

            // 1. فحص وتعقيم الحقول الإلزامية
            var label = ValidateLabel(request.Label);
            var addressType = ValidateAddressType(request.AddressType);
            var buildingName = ValidateBuildingName(request.BuildingName);

            // 2. تعقيم الحقول الاختيارية وقص المسافات الزائدة
            var floor = NormalizeOptionalString(request.Floor, 50, "الطابق");
            var doorInfo = NormalizeOptionalString(request.DoorInfo, 50, "معلومات الشقة/الباب");
            var notes = NormalizeOptionalString(request.Notes, 500, "الملاحظات");

            // إرجاع كائن الـ ValidatedInput النظيف والمطابق تماماً لخصائص الكلاس تبعك
            return new UpdateAddressValidatedInput
            (
                userId,
                addressId,
                label,
                addressType,
                buildingName,
                floor,
                doorInfo,
                notes
            );
        }

        // التحقق من التسمية
        private static string ValidateLabel(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("تسمية العنوان مطلوبة (مثل: المنزل، العمل).");

            value = value.Trim();
            if (value.Length > 50)
                throw new Exception("تسمية العنوان طويلة جداً. الحد الأقصى 50 حرفاً.");

            return value;
        }

        // التحقق من نوع العنوان (لاحظ أنه ليس Nullable هنا)
        private static AddressType ValidateAddressType(AddressType value)
        {
            if (!Enum.IsDefined(typeof(AddressType), value))
                throw new Exception("نوع العنوان المحدد غير صالح.");

            return value;
        }

        // التحقق من اسم البناية
        private static string ValidateBuildingName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("اسم أو رقم البناية مطلوب.");

            value = value.Trim();
            if (value.Length > 150)
                throw new Exception("اسم البناية طويل جداً. الحد الأقصى 150 حرفاً.");

            return value;
        }

        // ميثود مساعدة لتعقيم وتنظيف النصوص الاختيارية
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

    // الـ Record النظيف والمحمي الجاهز للتمرير إلى الـ Service
    public sealed record UpdateAddressValidatedInput
    (
        UserID UserID,
        AddressID AddressID,
        string Label,
        AddressType AddressType,
        string BuildingName,
        string? Floor,
        string? DoorInfo,
        string? Notes
    );
}

