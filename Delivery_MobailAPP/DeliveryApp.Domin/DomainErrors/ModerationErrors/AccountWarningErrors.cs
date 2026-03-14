using System;

namespace DeliveryApp.Domain.DomainErrors.ModerationErrors
{
    public static class AccountWarningErrors
    {
        public const string WarningAlreadyDecidedCode = "Warning_Already_Decided";
        public const string WarningAlreadyDecidedMessage = "The warning decision has already been made and cannot be modified.";

        public const string WarningAlreadyInactiveCode = "Warning_Already_Inactive";
        public const string WarningAlreadyInactiveMessage = "The warning is already inactive.";
    }
}