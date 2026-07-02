using DeliveryApp.Domain.Enums.IdentityEnums;

namespace DeliveryApp.Application.Features.Identity.Logout
{
    public static class LogoutValidator
    {
        public static LogoutValidatedInput Validate(LogoutRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            var deviceId = ValidateDeviceId(request.DeviceID);
            var clientType = ValidateClientType(request.ClientType);

            return new LogoutValidatedInput
            (
                deviceId,
                clientType
            );
        }

        // -------------------------
        //          DeviceId
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
        //         ClientType
        // -------------------------

        private static ClientType ValidateClientType(ClientType value)
        {
            if (!Enum.IsDefined(typeof(ClientType), value))
                throw new Exception("Invalid client type.");

            return value;
        }
    }

    public sealed record LogoutValidatedInput
    (
        string DeviceID,
        ClientType ClientType
    );
}