using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.DomainErrors
{
    internal class MerchantErrors
    {
        public const string PublicIdAlreadyAssignedCode = "Merchant.PublicId_AlreadyAssigned";
        public const string PublicIdAlreadyAssignedMessage = "PublicID is already assigned.";

        public const string ProfileIncompleteCode = "Merchant.Profile_Incomplete";
        public const string ProfileIncompleteMessage = "Store name, phone, logo, description, and location are required to activate profile.";

        public const string OwnerRequiredCode = "MerchantUser.Owner_Required";
        public const string OwnerRequiredMessage = "The owner cannot be removed without at least one replacement employee.";
    }
}
