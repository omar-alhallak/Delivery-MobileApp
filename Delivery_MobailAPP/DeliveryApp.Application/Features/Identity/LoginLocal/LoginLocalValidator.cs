using DeliveryApp.Domain.Enums.IdentityEnums;
using System.Text.RegularExpressions;

namespace DeliveryApp.Application.Features.Identity.LoginLocal
{
    public static partial class LoginLocalValidator
    {
        public static LoginLocalValidatedInput Validate(LoginLocalRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            var phone = ValidatePhone(request.Phone);
            var password = ValidatePassword(request.Password);
            var deviceId = ValidateDeviceId(request.DeviceID);
            var clientType = ValidateClientType(request.ClientType);

            return new LoginLocalValidatedInput(phone, password, deviceId, clientType);
        }

        // -------------------------
        // Phone
        // -------------------------

        private static string ValidatePhone(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) 
                throw new Exception("Phone is required.");

            value = NormalizeSpaces(value);

            if (!PhoneRegex().IsMatch(value)) 
                throw new Exception("Invalid phone format.");

            return value;
        }

        // -------------------------
        // Password
        // -------------------------

        private static string ValidatePassword(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Password is required.");

            return value;
        }

        // -------------------------
        // DeviceId
        // -------------------------

        private static string ValidateDeviceId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("DeviceId is required.");

            value = value.Trim();

            if (value.Length > 100)
                throw new Exception("DeviceId too long.");

            return value;
        }

        // -------------------------
        // ClientType
        // -------------------------

        private static ClientType ValidateClientType(ClientType value)
        {
            if (!Enum.IsDefined(typeof(ClientType), value))
                throw new Exception("Invalid client type.");

            return value;
        }

        // -------------------------
        // Helpers
        // -------------------------

        private static string NormalizeSpaces(string value) => SpaceRegex().Replace(value.Trim(), " ");

        // -------------------------
        // Regex
        // -------------------------

        [GeneratedRegex(@"^\+963 9\d{8}$")]
        private static partial Regex PhoneRegex();

        [GeneratedRegex(@"\s+")]
        private static partial Regex SpaceRegex();
    }

    public sealed record LoginLocalValidatedInput(string Phone, string Password, string DeviceID, ClientType ClientType);
}