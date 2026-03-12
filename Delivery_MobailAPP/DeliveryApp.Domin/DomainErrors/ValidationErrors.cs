using System;

namespace DeliveryApp.Domain.DomainErrors
{
    public static class ValidationErrors
    {
        // القيمة مطلوبة
        public const string RequiredCode = "Validation.Required";
        public const string RequiredMessage = "Value is required.";

        // نص أطول من الحد المسموح
        public const string TooLongCode = "Validation.Too_Long";
        public const string TooLongMessage = "Value is too long.";

        // رقم خارج المدى
        public const string OutOfRangeCode = "Validation.Out_Of_Range";
        public const string OutOfRangeMessage = "Value is out of range.";

        // Latitude out [90,-90] خط العرض
        public const string InvalidLatCode = "Validation.Invalid_Lat";
        public const string InvalidLatMessage = "Latitude must be between -90 and 90.";

        // Longitude out [180,-180] خط الطول
        public const string InvalidLngCode = "Validation.Invalid_Lng";
        public const string InvalidLngMessage = "Longitude must be between -180 and 180.";

        // Slug خاطئ
        public const string InvalidSlugCode = "Validation.Invalid_Slug";
        public const string InvalidSlugMessage = "Slug contains invalid characters.";

        // خطأ إدخال
        public const string InvalidFormatCode = "Validation.Invalid_Format";
        public const string InvalidFormatMessage = "Value has an invalid format.";
       
}
}