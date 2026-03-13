using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.DomainErrors.ModerationErrors
{
    public static class ComplaintErrors
    {
        //لايمكن تقديم شكوى ضد طلب تم تسليمه بنجاح
        //public const string ComplaintAlreadyClosedCode = "COMPLAINT_ALREADY_CLOSED";
        //public const string ComplaintAlreadyClosedMessage = "The complaint is already closed and cannot be modified.";
        //الطلبات المعلقة لا يمكن تعديلها 
        public const string ComplaintMustBePendingCode = "COMPLAINT_MUST_BE_PENDING";
        public const string ComplaintMustBePendingMessage = "Only pending complaints can be modified or reviewed.";
        //لا يمكن تقديم طلب 
        public const string ComplaintCannotTargetSelfCode = "COMPLAINT_CANNOT_TARGET_SELF";
        public const string ComplaintCannotTargetSelfMessage = "A user cannot create a complaint against themselves.";
        
        public const string AdminResponseRequiredCode = "COMPLAINT_ADMIN_RESPONSE_REQUIRED";
        public const string AdminResponseRequiredMessage = "Admin response is required when closing the complaint.";
    }
}
