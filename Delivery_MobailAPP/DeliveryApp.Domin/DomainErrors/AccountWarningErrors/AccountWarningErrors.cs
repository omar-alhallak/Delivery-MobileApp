using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.DomainErrors.AccountWarningErrors
{
    public static class AccountWarningErrors
    {
        //تم اتخاذ القرار بالفعل ولا يمكن تعديله
        public const string WarningAlreadyDecidedCode = "WARNING_ALREADY_DECIDED";
        public const string WarningAlreadyDecidedMessage = "The warning decision has already been made and cannot be modified.";
        // لا يمكن إلغاء تحذير غير نشط
        public const string WarningAlreadyInactiveCode = "WARNING_ALREADY_INACTIVE";
        public const string WarningAlreadyInactiveMessage = "The warning is already inactive.";
        // يجب تقديم قرار صالح عند اتخاذ قرار بشأن التحذير
        public const string WarningDecisionRequiredCode = "WARNING_DECISION_REQUIRED";
        public const string WarningDecisionRequiredMessage = "A valid decision must be provided.";
        // قرار تحذير غير صالح
        public const string InvalidWarningDecisionCode = "INVALID_WARNING_DECISION";
        public const string InvalidWarningDecisionMessage = "Invalid warning decision.";
        // لا يمكن تعديل تحذير غير نشط
        public const string CannotModifyInactiveWarningCode = "WARNING_INACTIVE";
        public const string CannotModifyInactiveWarningMessage = "Inactive warning cannot be modified.";
    }
}
