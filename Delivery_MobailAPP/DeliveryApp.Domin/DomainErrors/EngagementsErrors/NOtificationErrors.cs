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

        public const string TitleTooLongCode = "Notification.TitleTooLong";
        public const string TitleTooLongMessage = "Title exceeds the maximum allowed length (150).";

        // Body
        public const string InvalidBodyCode = "Notification.InvalidBody";
        public const string InvalidBodyMessage = "Notification body is required and cannot be empty.";

        public const string BodyTooLongCode = "Notification.BodyTooLong";
        public const string BodyTooLongMessage = "Body exceeds the maximum allowed length (1000).";

        // Type & Logic
        public const string InvalidTypeCode = "Notification.InvalidType";
        public const string InvalidTypeMessage = "Notification type must be a valid value.";

        public const string RelatedEntityRequiredCode = "Notification.RelatedEntityRequired";
        public const string RelatedEntityRequiredMessage = "RelatedEntityID is required for this notification type.";
    }
}
