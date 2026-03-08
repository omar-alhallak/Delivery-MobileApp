using System;

namespace DeliveryApp.Domain.DomainErrors.IdentityErrors
{
    public static class UserSessionErrors
    {
        public const string DeviceMismatchCode = "User_Session.Device_Mismatch";
        public const string DeviceMismatchMessage = "Account is already logged in on another device.";

        public const string SessionRevokedCode = "User_Session.Revoked";
        public const string SessionRevokedMessage = "Session is revoked. Please login again.";

        public const string SessionExpiredCode = "User_Session.Expired";
        public const string SessionExpiredMessage = "Session is expired. Please login again.";

        public const string RevokedAtBeforeCreatedAtCode = "User_Session.Revoked_At_Before_Created_At";
        public const string RevokedAtBeforeCreatedAtMessage = "RevokedAt cannot be earlier than CreatedAt.";
    }
}