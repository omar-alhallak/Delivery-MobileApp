namespace DeliveryApp.Domain.DomainErrors.MerchantErrors
{
    public static class MerchantWorkingHourErrors
    {
        // المحل مغلق لا يمكن إضافة أوقات عمل
        public const string ClosedDayCantHaveWorkingHoursCode = "MerchantWorkingHour.Closed_Day_Cant_Have_Working_Hours";
        public const string ClosedDayCantHaveWorkingHoursMessage = "Closed day cant have opening or closing time.";

        // المطعم مفتوح على 24 الساعة لا يمكنك تحديد أوقات عمل له
        public const string OpenAllDayCantHaveSpecificHoursCode = "MerchantWorkingHour.Open_All_Day_Cant_Have_Specific_Hours";
        public const string OpenAllDayCantHaveSpecificHoursMessage = "Open all day cant have specific opening or closing time.";

        // وقت الفتح = وقت الإغلاق
        public const string InvalidWorkingHoursCode = "MerchantWorkingHour.Invalid_Working_Hours";
        public const string InvalidWorkingHoursMessage = "Opening time and closing time cant be the same.";

        // وقت الإغلاق قبل وقت الفتح
        public const string OpenTimeMustBeBeforeCloseTimeCode = "MerchantWorkingHour.Open_Time_Must_Be_Before_Close_Time";
        public const string OpenTimeMustBeBeforeCloseTimeMessage = "Opening time must be before closing time.";
    }
}