namespace DeliveryApp.Domain.DomainErrors.IdentityErrors
{
    public static class UserIdentityErrors
    {
        // كلمة السر مطلوبة عن طريق تسجيل المحلي
        public const string PasswordRequiredForLocalCode = "User_Identity.Password_Required_For_Local";
        public const string PasswordRequiredForLocalMessage = "PasswordHash is required for Local provider.";

        // Sup مطلوبة عن طريق تسجيل بغوغل
        public const string GoogleSubRequiredCode = "User_Identity.Google_Sub_Required";
        public const string GoogleSubRequiredMessage = "GoogleSub (ProviderUserID) is required.";

        // لا يمكن تغيير كلمة السر إلا عن طريق تسجيل المحلي
        public const string PasswordChangeOnlyForLocalCode = "User_Identity.Password_Change_Only_For_Local";
        public const string PasswordChangeOnlyForLocalMessage = "Password can be changed only for Local provider.";

        // لا يمكن للتسجيل المحلي أن يحوي على Provider
        public const string LocalCantBeHaveProviderUserIDCode = "User_Identity.Local_Must_Not_Have_Provider_User_ID";
        public const string LocalCantBeHaveProviderUserIDMessage = "Local identity must not have ProviderUserID.";

        // لا يمكن للتسجيل المحلي أن يحوي على كلمة سر
        public const string GoogleCantBeHavePasswordHashCode = "User_Identity.Google_Must_Not_Have_Password_Hash";
        public const string GoogleCantBeHavePasswordHashMessage = "Google identity must not have PasswordHash.";

        // Provider غير مدعوم
        public const string UnsupportedProviderCode = "User_Identity.Unsupported_Provider";
        public const string UnsupportedProviderMessage = "Unsupported auth provider.";
    }
}