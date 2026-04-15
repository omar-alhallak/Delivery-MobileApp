using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.MerchantErrors;

namespace DeliveryApp.Domain.Entities.Merchants
{
    public class MerchantWorkingHour // يمثل ساعات عمل التاجر لكل يوم من أيام الأسبوع
    {
        // -------------------------
        //            Key
        // -------------------------

        public MerchantWorkingHourID ID { get; private set; } // PK معرف سجل ساعات العمل

        // -------------------------
        //      Relation Keys
        // -------------------------

        public MerchantID MerchantID { get; private set; } // معرف التاجر المرتبط بهذا السجل

        // -------------------------
        //        Working Day
        // -------------------------

        public DayOfWeek Day { get; private set; } // اليوم الدوام

        // -------------------------
        //      Working Hours
        // -------------------------

        public TimeOnly? OpenTime { get; private set; } // وقت الفتح
        public TimeOnly? CloseTime { get; private set; } // وقت الإغلاق
        public bool IsClosed { get; private set; } // هل هذا اليوم مغلق بالكامل
        public bool IsOpenAllDay { get; private set; } // هل يعمل 24 ساعة

        private MerchantWorkingHour() { }

        public MerchantWorkingHour(MerchantWorkingHourID id, MerchantID merchantId, DayOfWeek dayOfWeek, TimeOnly? openTime, TimeOnly? closeTime, bool isClosed, bool isOpenAllDay)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (merchantId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(merchantId));

            if (!Enum.IsDefined(typeof(DayOfWeek), dayOfWeek)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(dayOfWeek));

            ValidateWorkingHours(openTime, closeTime, isClosed, isOpenAllDay);

            ID = id;
            MerchantID = merchantId;
            Day = dayOfWeek;
            OpenTime = openTime;
            CloseTime = closeTime;
            IsClosed = isClosed;
            IsOpenAllDay = isOpenAllDay;
        }

        // -------------------------
        //        Behaviors
        // -------------------------

        public void UpdateWorkingHours(TimeOnly? openTime, TimeOnly? closeTime, bool isClosed, bool isOpenAllDay) // تحديث ساعات العمل اليوم
        {
            if (OpenTime == openTime && CloseTime == closeTime && IsClosed == isClosed && IsOpenAllDay == isOpenAllDay) return;

            ValidateWorkingHours(openTime, closeTime, isClosed, isOpenAllDay);

            OpenTime = openTime;
            CloseTime = closeTime;
            IsClosed = isClosed;
            IsOpenAllDay = isOpenAllDay;
        }


        // -------------------------
        //       Validations
        // -------------------------
        private static void ValidateWorkingHours(TimeOnly? openTime, TimeOnly? closeTime, bool isClosed, bool isOpenAllDay) // التحقق من صحة أوقات العمل
        {
            if (isClosed && isOpenAllDay) throw new DomainRuleViolationException
                    (MerchantWorkingHourErrors.InvalidWorkingHoursCode, MerchantWorkingHourErrors.InvalidWorkingHoursMessage);

            // مغلق بالكامل
            if (isClosed)
            {
                if (openTime is not null || closeTime is not null) throw new DomainRuleViolationException
                        (MerchantWorkingHourErrors.ClosedDayCantHaveWorkingHoursCode, MerchantWorkingHourErrors.ClosedDayCantHaveWorkingHoursMessage);

                return;
            }

            // مفتوح 24 ساعة
            if (isOpenAllDay)
            {
                if (openTime is not null || closeTime is not null) throw new DomainRuleViolationException
                        (MerchantWorkingHourErrors.OpenAllDayCantHaveSpecificHoursCode, MerchantWorkingHourErrors.OpenAllDayCantHaveSpecificHoursMessage);

                return;
            }

            if (openTime is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(openTime));

            if (closeTime is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(closeTime));

            if (openTime == closeTime) throw new DomainValidationException
                    (MerchantWorkingHourErrors.InvalidWorkingHoursCode, MerchantWorkingHourErrors.InvalidWorkingHoursMessage, nameof(openTime));
        }
    }
}