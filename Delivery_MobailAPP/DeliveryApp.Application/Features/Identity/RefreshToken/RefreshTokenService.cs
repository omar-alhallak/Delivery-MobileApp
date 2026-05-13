using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Application.Interfaces.IdentityInterfaces;

namespace DeliveryApp.Application.Features.Identity.RefreshToken
{
    public sealed class RefreshTokenService
    {
        private readonly IRefreshTokenRepository _repository;
        private readonly ITokenService _tokenService;

        public RefreshTokenService(
            IRefreshTokenRepository repository,
            ITokenService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }

        public async Task<RefreshTokenResponse> ExecuteAsync(
            RefreshTokenRequest request,
            CancellationToken ct = default)
        {
            var now = DateTimeOffset.UtcNow;
            var sessionLifetime = _tokenService.GetRefreshTokenLifetime();

            // -------------------------
            //        Validation
            // -------------------------

            var input = RefreshTokenValidator.Validate(request);

            // -------------------------
            //          User
            // -------------------------

            var user = await _repository.GetUserByIdAsync(
                UserID.From(input.UserId),
                ct);

            if (user is null)
                throw new Exception("User not found.");

            // -------------------------
            //         Session
            // -------------------------

            var session = await _repository.GetSessionAsync(
                user.ID,
                input.ClientType,
                ct);

            if (session is null)
                throw new Exception("Session not found.");

            // -------------------------
            //      Verify Device
            // -------------------------

            session.EnsureSameDevice(input.DeviceID);

            // -------------------------
            //    Verify RefreshToken
            // -------------------------

            var refreshTokenHash =
                _tokenService.HashRefreshToken(input.RefreshToken);

            var matches =
                session.MatchesRefreshTokenHash(refreshTokenHash);

            if (!matches)
                throw new Exception("Invalid refresh token.");

            // -------------------------
            //      Ensure Active
            // -------------------------

            if (!session.IsActive(now))
                throw new Exception("Session expired.");

            // -------------------------
            //       New Tokens
            // -------------------------

            var newAccessToken =
                _tokenService.GenerateAccessToken(user);

            var newRefreshToken =
                _tokenService.GenerateRefreshToken();

            var newRefreshTokenHash =
                _tokenService.HashRefreshToken(newRefreshToken);

            // -------------------------
            //      Refresh Session
            // -------------------------

            session.Refresh(
                deviceId: input.DeviceID,
                newRefreshTokenHash: newRefreshTokenHash,
                utcNow: now,
                lifetime: sessionLifetime);

            // -------------------------
            //            Save
            // -------------------------

            await _repository.SaveChangesAsync(ct);

            // -------------------------
            //         Response
            // -------------------------

            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}