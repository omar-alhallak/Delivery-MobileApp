namespace DeliveryApp.Domain.DomainErrors.MerchantErrors
{
    public static class MerchantErrors
    {
        // تم تعيين المعرف مسبقاً
        public const string PublicIdAlreadyAssignedCode = "Merchant.PublicId_Already_Assigned";
        public const string PublicIdAlreadyAssignedMessage = "Public ID is already assigned.";

        // لا يمكنك تفعيل المطعم إذا ماله أسم
        public const string CantActivateWithoutNameCode = "Merchant.Cant_Activate_Without_Name";
        public const string CantActivateWithoutNameMessage = "Merchant cant be activated without a name.";

        // لا يمكنك تفعيل المطعم إذا ماله لوغو
        public const string CantActivateWithoutLogoCode = "Merchant.Cant_Activate_Without_Logo";
        public const string CantActivateWithoutLogoMessage = "Merchant cant be activated without a logo image.";

        // لا يمكنك تفعيل المطعم إذا ماله صورة
        public const string CantActivateWithoutCoverImageCode = "Merchant.Cant_Activate_Without_Cover_Image";
        public const string CantActivateWithoutCoverImageMessage = "Merchant cant be activated without a cover image.";

        public const string PreparingTimeTooSmallCode = "Merchant.PreparingTimeTooSmall";
        public const string PreparingTimeTooSmallMessage = "Preparing time must be at least 1 minute.";

        public const string PreparingTimeTooLargeCode = "Merchant.PreparingTimeTooLarge";
        public const string PreparingTimeTooLargeMessage = "Preparing time is too large.";
    }
}