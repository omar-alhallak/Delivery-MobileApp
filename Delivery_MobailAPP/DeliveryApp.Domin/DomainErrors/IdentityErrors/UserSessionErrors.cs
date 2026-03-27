namespace DeliveryApp.Domain.DomainErrors.IdentityErrors
{
    public static class UserSessionErrors
    {
        // المستخدم مسجل دخول بجهاز اخر
        public const string DeviceMismatchCode = "User_Session.Device_Mismatch";
        public const string DeviceMismatchMessage = "Account is already logged in on another device.";

        // تم إلغاء الجلسة
        public const string SessionRevokedCode = "User_Session.Revoked";
        public const string SessionRevokedMessage = "Session is revoked. Please login again.";

        // انتهى صلاحية الجلسة
        public const string SessionExpiredCode = "User_Session.Expired";
        public const string SessionExpiredMessage = "Session is expired. Please login again.";

        // وقن الإنشاء بعد وقت الإلغاء
        public const string RevokedAtBeforeCreatedAtCode = "User_Session.Revoked_At_Before_Created_At";
        public const string RevokedAtBeforeCreatedAtMessage = "RevokedAt cant be earlier than CreatedAt.";
    }
}