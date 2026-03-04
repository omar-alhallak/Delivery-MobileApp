using System;

namespace DeliveryApp.Domain.DomainErrors.IdentityErrors
{
    public static class UserIdentityErrors
    {
        public const string PasswordRequiredForLocalCode = "User_Identity.password_Required_For_Local";
        public const string PasswordRequiredForLocalMessage = "PasswordHash is required for Local provider.";

        public const string GoogleSubRequiredCode = "User_Identity.Google_Sub_Required";
        public const string GoogleSubRequiredMessage = "GoogleSub (ProviderUserID) is required.";

        public const string PasswordChangeOnlyForLocalCode = "User_Identity.password_Change_Only_For_Local";
        public const string PasswordChangeOnlyForLocalMessage = "Password can be changed only for Local provider.";

        public const string LocalCantBeHaveProviderUserIDCode = "User_Identity.local_Must_not_have_Provider_User_ID";
        public const string LocalCantBeHaveProviderUserIDMessage = "Local identity must not have ProviderUserID.";

        public const string GoogleCantBeHavePasswordHashCode = "User_Identity.google_Must_not_have_Password_Hash";
        public const string GoogleCantBeHavePasswordHashMessage = "Google identity must not have PasswordHash.";

        public const string UnsupportedProviderCode = "User_Identity.unsupported_Provider";
        public const string UnsupportedProviderMessage = "Unsupported auth provider.";
    }
}