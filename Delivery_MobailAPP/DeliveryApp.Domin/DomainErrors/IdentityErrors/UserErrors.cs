namespace DeliveryApp.Domain.DomainErrors.IdentityErrors
{
    public static class UserErrors
    {
        // تم إضافة المعرف مسبقاً
        public const string PublicIdAlreadyAssignedCode = "User.Public_ID_Already_Assigned";
        public const string PublicIdAlreadyAssignedMessage = "Public ID is already assigned.";

        // لا بمكنك إنشائ الحساب هناك حقول غير مكتملة
        public const string ProfileFieldNotCompleteCode = "User.Profile_Field_NotComplete";
        public const string ProfileFieldNotCompleteMessage = "Cant complete The profile. Phone and FullName are required.";

        // لا يمكنك حذف حقب مطلوب
        public const string CantRemoveRequiredFieldCode = "User.Profile_Cant_Remove_Required_Field";
        public const string CantRemoveRequiredFieldMessage = "Profile is complete; Cant remove a required field.";

        // لا يمكنك إذالة صلاحية زبون من السائق
        public const string CantRemoveCustFromDrivCode = "User.Cant_Remove_Customer_From_Driver";
        public const string CantRemoveCustFromDrivMessage = "Cant remove Customer role from a Driver.";

        // وقت التعليق الحساب يجب أن يكون في المستقبل
        public const string SuspensionMustBeFutureCode = "User.Suspension_Must_Be_Future";
        public const string SuspensionMustBeFutureMessage = "Suspension time must be in the future.";

        // لا يمكنك تعديل حساب مبند
        public const string BannedCantBeModifiedCode = "User.Banned_Cant_Be_Modified";
        public const string BannedCantBeModifiedMessage = "Banned user cant be modified.";
    }
}