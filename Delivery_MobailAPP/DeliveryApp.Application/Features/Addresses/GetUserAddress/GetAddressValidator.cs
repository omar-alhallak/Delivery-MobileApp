using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Application.Features.Addresses.GetUserAddress
{
    public static class GetAddressValidator
    {

        public static GetAddressValidatedInput ValidateSingle(UserID userId, GetUserAddressRequest request)
        {
            if (request is null)
                throw new Exception("بيانات طلب جلب العنوان مطلوبة.");

            if (request.addressID == default)
                throw new Exception("معرّف العنوان المطلوب غير صالح أو مفقود.");

            return new GetAddressValidatedInput(userId, request.addressID);
        }

        // 2. ميثود جلب قائمة العناوين (تتأكد فقط من الـ UserID)
        public static void ValidateList(UserID userId)
        {
            if (userId == default)
                throw new Exception("معرّف المستخدم غير صالح.");
        }

        // الـ Record النظيف والمحمي لجلب عنوان واحد
        public sealed record GetAddressValidatedInput(UserID UserID, AddressID AddressID);
    }
}

