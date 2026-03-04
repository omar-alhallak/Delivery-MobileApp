using System;

namespace DeliveryApp.Domain.DomainErrors.IdentityErrors
{
    public static class UserErrors
    {
        public const string PublicIdAlreadyAssignedCode = "User.public_ID_Already_Assigned";
        public const string PublicIdAlreadyAssignedMessage = "Public ID is already assigned.";

        public const string ProfileFieldNotCompleteCode = "User.profile_Field_NotComplete";
        public const string ProfileFieldNotCompleteMessage = "Cannot complete The profile. Phone and FullName are required.";

        public const string CantRemoveRequiredFieldCode = "User.profile_Cannot_Remove_Required_Field";
        public const string CantRemoveRequiredFieldMessage = "Profile is complete; Cannot remove a required field.";

        public const string CantRemoveCustFromDrivCode = "User.cannot_Remove_Customer_From_Driver";
        public const string CantRemoveCustFromDrivMessage = "Cannot remove Customer role from a Driver.";

        public const string SuspensionMustBeFutureCode = "User.suspension_Must_be_Future";
        public const string SuspensionMustBeFutureMessage = "Suspension time must be in the future.";

        public const string BannedCannotBeModifiedCode = "User.banned_Cannot_be_Modified";
        public const string BannedCannotBeModifiedMessage = "Banned user cannot be modified.";
    }
}