using DeliveryApp.Application.Interfaces.IdentityInterfaces;

namespace DeliveryApp.Application.Features.Identity.Logout
{
    public sealed class LogoutService
    {
        private readonly ILogoutRepository _repository;

        public LogoutService(ILogoutRepository repository)
        {
            _repository = repository;
        }

        public async Task<LogoutResponse> ExecuteAsync(
            Guid userId,
            LogoutRequest request,
            CancellationToken ct = default)
        {
            var userID = UserID.From(userId);
            var now = DateTimeOffset.UtcNow;

            // -------------------------
            //        Validation
            // -------------------------

            var input = LogoutValidator.Validate(request);

            // -------------------------
            //          Session
            // -------------------------

            var session = await _repository.GetSessionAsync(
                userID,
                input.ClientType,
                ct);

            if (session is null)
                throw new Exception("Session not found.");

            // -------------------------
            //      Verify Device
            // -------------------------

            session.EnsureSameDevice(input.DeviceID);

            // -------------------------
            //          Revoke
            // -------------------------

            session.Revoke(now);

            // -------------------------
            //            Save
            // -------------------------

            await _repository.SaveChangesAsync(ct);

            // -------------------------
            //         Response
            // -------------------------

            return new LogoutResponse
            {
                Success = true
            };
        }
    }
}