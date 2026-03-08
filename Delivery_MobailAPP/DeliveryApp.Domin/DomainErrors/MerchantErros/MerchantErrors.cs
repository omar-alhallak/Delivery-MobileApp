using System;

namespace DeliveryApp.Domain.DomainErrors.MerchantErrors
{
    public static class MerchantErrors
    {
        public const string PublicIdAlreadyAssignedCode = "Merchant.PublicId_Already_Assigned";
        public const string PublicIdAlreadyAssignedMessage = "Public ID is already assigned.";

        public const string CantActivateWithoutNameCode = "Merchant.Cannot_Activate_Without_Name";
        public const string CantActivateWithoutNameMessage = "Merchant cannot be activated without a name.";

        public const string CantActivateWithoutLogoCode = "Merchant.Cannot_Activate_Without_Logo";
        public const string CantActivateWithoutLogoMessage = "Merchant cannot be activated without a logo image.";

        public const string CantActivateWithoutCoverImageCode = "Merchant.Cannot_Activate_Without_Cover_Image";
        public const string CantActivateWithoutCoverImageMessage = "Merchant cannot be activated without a cover image.";
    }
}