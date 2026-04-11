using DeliveryApp.Domain.ValueObjects;
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

        public DayOfWeek Day { get; private set; } // اليوم الذي ينطبق عليه الدوام

        // -------------------------
        //      Working Hours
        // -------------------------

        public TimeOnly? OpenTime { get; private set; } // وقت الفتح
        public TimeOnly? CloseTime { get; private set; } // وقت الإغلاق
        public bool IsClosed { get; private set; } // هل هذا اليوم مغلق بالكامل

        private MerchantWorkingHour() { }

        public MerchantWorkingHour(
            MerchantWorkingHourID id,
            MerchantID merchantId,
            DayOfWeek dayOfWeek,
            TimeOnly? openTime,
            TimeOnly? closeTime,
            bool isClosed)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (merchantId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(merchantId));

            ValidateWorkingHours(openTime, closeTime, isClosed);

            ID = id;
            MerchantID = merchantId;
            Day = dayOfWeek;
            OpenTime = openTime;
            CloseTime = closeTime;
            IsClosed = isClosed;
        }

        // -------------------------
        //        Behaviors
        // -------------------------

        public void UpdateWorkingHours(TimeOnly? openTime, TimeOnly? closeTime, bool isClosed) // تحديث ساعات العمل لليوم
        {
            ValidateWorkingHours(openTime, closeTime, isClosed);

            OpenTime = openTime;
            CloseTime = closeTime;
            IsClosed = isClosed;
        }


        // -------------------------
        //       Validations
        // -------------------------
        private static void ValidateWorkingHours(TimeOnly? openTime, TimeOnly? closeTime, bool isClosed) // التحقق من صحة أوقات العمل
        {
            if (isClosed)
            {
                if (openTime is not null || closeTime is not null) throw new DomainRuleViolationException
                        (MerchantWorkingHourErrors.ClosedDayCannotHaveWorkingHoursCode, MerchantWorkingHourErrors.ClosedDayCannotHaveWorkingHoursMessage);

                return;
            }

            if (openTime is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(openTime));

            if (closeTime is null) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(closeTime));

            if (openTime.Value >= closeTime.Value) throw new DomainValidationException
                    (MerchantWorkingHourErrors.OpenTimeMustBeBeforeCloseTimeCode, MerchantWorkingHourErrors.OpenTimeMustBeBeforeCloseTimeMessage, nameof(openTime));
        }
    }
}