using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.DomainErrors.EngagementsErrors
{
    internal class NotificationErrors
    {
        public const string InvalidTitleCode = "Notification.InvalidTitle";
        public const string InvalidTitleMessage = "Title cannot be empty or whitespace.";

        public const string TooLongCode = "Notification.TooLong";
        public const string TooLongMessage = "Content is too long. Please stay within the character limit.";

        // Body Errors
        public const string InvalidBodyCode = "Notification.InvalidBody";
        public const string InvalidBodyMessage = "Notification body is required and cannot be empty.";

        public const string IncompatibleTypeCode = "Notification.IncompatibleType";
        public const string IncompatibleTypeMessage = "The notification type does not match the requested action.";

        public const string EmptyBodyCode = "Notification.Empty_Body";
        public const string EmptyBodyMessage = "Notification body cannot be empty.";

        public const string BodyTooLongCode = "Notification.Body_Too_Long";
        public const string BodyTooLongMessage = "Body exceeds the maximum allowed length (1000).";

        // أخطاء النوع والارتباط
        public const string InvalidTypeCode = "Notification.Invalid_Type";
        public const string InvalidTypeMessage = "Notification type must be a valid positive value.";

        public const string RelatedEntityRequiredCode = "Notification.Related_Entity_Required";
        public const string RelatedEntityRequiredMessage = "RelatedEntityID is required when type is specified.";
    }
}
