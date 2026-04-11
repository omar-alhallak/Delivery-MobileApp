namespace DeliveryApp.Domain.DomainErrors.MerchantErrors
{
    public static class MerchantWorkingHourErrors
    {
        public const string ClosedDayCannotHaveWorkingHoursCode = "MerchantWorkingHour.ClosedDayCannotHaveWorkingHours";
        public const string ClosedDayCannotHaveWorkingHoursMessage = "Closed day cannot have open or close time.";

        public const string OpenTimeMustBeBeforeCloseTimeCode = "MerchantWorkingHour.OpenTimeMustBeBeforeCloseTime";
        public const string OpenTimeMustBeBeforeCloseTimeMessage = "Open time must be before close time.";
    }
}   