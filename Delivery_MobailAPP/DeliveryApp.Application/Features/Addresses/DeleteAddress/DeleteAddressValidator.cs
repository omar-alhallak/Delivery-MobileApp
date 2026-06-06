using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Application.Features.Addresses.DeleteAddress
{
    public class DeleteAddressValidator
    {

            public static DeleteAddressValidatedInput Validate(UserID userId, DeleteAddressRequest request)
            {
                if (request is null)
                    throw new Exception("بيانات طلب الحذف مطلوبة.");

                // التحقق من أن معرف العنوان المرسل ليس default (لأنه Strongly Typed ID)
                if (request.ID == default)
                    throw new Exception("معرّف العنوان المراد حذفه غير صالح أو مفقود.");

                // إرجاع كائن الحذف المعقم والمحمي بالكامل
                return new DeleteAddressValidatedInput
                (
                    userId,
                    request.ID
                );
            }
        

        // الـ Record النظيف والمحمي الجاهز للتمرير إلى الـ Service
        public sealed record DeleteAddressValidatedInput
        (
            UserID UserID,
            AddressID AddressID
        );
    }
}
