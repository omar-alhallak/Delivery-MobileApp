using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Enums.IdentityEnums;
using DeliveryApp.Application.Interfaces.Services;
using DeliveryApp.Application.Interfaces.IdentityRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Identity.LoginLocal
{
    public sealed class LoginLocalService
    {
        private readonly ILoginLocalRepository _repository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public LoginLocalService(ILoginLocalRepository repository, IPasswordHasher passwordHasher, ITokenService tokenService)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<LoginLocalResponse> ExecuteAsync(LoginLocalRequest request, CancellationToken ct = default)
        {
            // تحديد مدة الجلسة
            var now = DateTimeOffset.UtcNow;
            var sessionLifetime = _tokenService.GetRefreshTokenLifetime();

            // -------------------------
            //        Validation
            // -------------------------

            var input = LoginLocalValidator.Validate(request);

            // -------------------------
            //          Get User
            // -------------------------

            var user = await _repository.GetUserByPhoneAsync(input.Phone, ct);

            if (user is null)
                throw new Exception("Phone or password is incorrect.");

            // -------------------------
            //       Get Identity
            // -------------------------

            var identity = await _repository.GetLocalIdentityAsync(user.ID, ct);

            if (identity is null)
                throw new Exception("Phone or password is incorrect.");

            // -------------------------
            //      Verify Password
            // -------------------------

            var isValid = _passwordHasher.Verify(input.Password, identity.PasswordHash!);

            if (!isValid)
                throw new Exception("Phone or password is incorrect.");

            // -------------------------
            //       Check Status
            // -------------------------

            if (user.AccountStatus == AccountStatus.Banned)
                throw new Exception("Account is banned.");

            user.AutoActivateIfExpired(now);

            if (user.AccountStatus == AccountStatus.Suspended)
                throw new Exception("Account is suspended.");

            // -------------------------
            //       Merchant Access
            // -------------------------

            if (input.ClientType == ClientType.MerchantWebApp)
            {
                var hasMerchantAccess = await _repository.HasMerchantAccessAsync(user.ID, ct);

                if (!hasMerchantAccess) throw new Exception("Merchant access denied.");    
            }

            // -------------------------
            //          Tokens
            // -------------------------

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);

            // -------------------------
            //          Session
            // -------------------------

            var session = await _repository.GetSessionAsync(user.ID, input.ClientType, ct);

            if (session is null)
            {
                session = UserSession.Create
                (
                    id: UserSessionID.New(),
                    userId: user.ID,
                    clientType: input.ClientType,
                    deviceId: input.DeviceID,
                    refreshTokenHash: refreshTokenHash,
                    utcNow: now,
                    lifetime: sessionLifetime
                );

                await _repository.AddSessionAsync(session, ct);
            }
            else if (session.IsRevoked)
            {
                _repository.RemoveSession(session);

                session = UserSession.Create
                (
                    id: UserSessionID.New(),
                    userId: user.ID,
                    clientType: input.ClientType,
                    deviceId: input.DeviceID,
                    refreshTokenHash: refreshTokenHash,
                    utcNow: now,
                    lifetime: sessionLifetime
                );

                await _repository.AddSessionAsync(session, ct);
            }
            else
            {
                session.Refresh
                (
                    deviceId: input.DeviceID,
                    newRefreshTokenHash: refreshTokenHash,
                    utcNow: now,
                    lifetime: sessionLifetime
                );
            }

            // -------------------------
            //            Save
            // -------------------------

            await _repository.SaveChangesAsync(ct);

            // -------------------------
            //         Response
            // -------------------------

            return new LoginLocalResponse
            {
                UserId = user.ID.Value,
                PublicId = user.PublicID!.Value.Value,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IsProfileComplete = user.IsProfileComplete
            };
        }
    }
}