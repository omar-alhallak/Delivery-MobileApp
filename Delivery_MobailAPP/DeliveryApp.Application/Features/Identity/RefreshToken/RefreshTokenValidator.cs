using DeliveryApp.Domain.Enums.IdentityEnums;

namespace DeliveryApp.Application.Features.Identity.RefreshToken
{
    public static class RefreshTokenValidator
    {
        public static RefreshTokenValidatedInput Validate(RefreshTokenRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            var userId = ValidateUserId(request.UserId);
            var refreshToken = ValidateRefreshToken(request.RefreshToken);
            var deviceId = ValidateDeviceId(request.DeviceID);
            var clientType = ValidateClientType(request.ClientType);

            return new RefreshTokenValidatedInput(
                userId,
                refreshToken,
                deviceId,
                clientType);
        }

        // -------------------------
        //          UserId
        // -------------------------

        private static Guid ValidateUserId(Guid value)
        {
            if (value == Guid.Empty)
                throw new Exception("User id is required.");

            return value;
        }

        // -------------------------
        //       RefreshToken
        // -------------------------

        private static string ValidateRefreshToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Refresh token is required.");

            return value.Trim();
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

    public sealed record RefreshTokenValidatedInput(
        Guid UserId,
        string RefreshToken,
        string DeviceID,
        ClientType ClientType);
}