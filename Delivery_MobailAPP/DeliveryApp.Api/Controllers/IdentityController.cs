using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using DeliveryApp.Application.Features.Identity.Logout;
using DeliveryApp.Application.Features.Identity.LoginLocal;
using DeliveryApp.Application.Features.Identity.RefreshToken;
using DeliveryApp.Application.Features.Identity.RegisterLocal;
using DeliveryApp.Application.Features.Identity.UpdateMyProfile;
using DeliveryApp.Application.Features.Identity.GetMyProfile;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Route("api/identity")]
    public sealed class IdentityController : ControllerBase
    {
        private readonly RegisterLocalService _registerLocalService;
        private readonly LoginLocalService _loginLocalService;
        private readonly UpdateMyProfileService _updateMyProfileService;
        private readonly RefreshTokenService _refreshTokenService;
        private readonly LogoutService _logoutService;
        private readonly GetMyProfileService _getMyProfileService;

        public IdentityController(RegisterLocalService registerLocalService, LoginLocalService loginLocalService, UpdateMyProfileService updateMyProfileService, RefreshTokenService refreshTokenService, LogoutService logoutService, GetMyProfileService getMyProfileService)
        {
            _registerLocalService = registerLocalService ?? throw new ArgumentNullException(nameof(registerLocalService));
            _loginLocalService = loginLocalService;
            _updateMyProfileService = updateMyProfileService;
            _refreshTokenService = refreshTokenService;
            _logoutService = logoutService;
            _getMyProfileService = getMyProfileService;
        }

        // -------------------------
        //          EndPoint
        // ------------------------- 

        // Endpoint Register
        [HttpPost("register/local")]
        public async Task<ActionResult<RegisterLocalResponse>> RegisterLocal([FromBody] RegisterLocalRequest request, CancellationToken ct)
        {
            var response = await _registerLocalService.ExecuteAsync(request, ct);

            return Ok(response);
        }

        // Endpoint Login
        [HttpPost("login/local")]
        public async Task<ActionResult<LoginLocalResponse>> LoginLocal([FromBody] LoginLocalRequest request, CancellationToken ct)
        {
            var response = await _loginLocalService.ExecuteAsync(request, ct);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("my-profile")]
        public async Task<ActionResult<GetMyProfileResponse>> Me(CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _getMyProfileService.ExecuteAsync(userId, ct);

            return Ok(response);
        }

        // Endpoint Put my-profile
        // لتحديث معلومات الحساب
        [Authorize]
        [HttpPut("my-profile")]
        public async Task<ActionResult<UpdateMyProfileResponse>> UpdateMyProfile([FromBody] UpdateMyProfileRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _updateMyProfileService.ExecuteAsync(userId, request, ct);

            return Ok(response);
        }

        // تجديد التوكنات
        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
        {
            var response = await _refreshTokenService.ExecuteAsync(request, ct);

            return Ok(response);
        }

        // Endpoint Logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<LogoutResponse>> Logout([FromBody] LogoutRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _logoutService.ExecuteAsync(userId, request, ct);

            return Ok(response);
        }

        // -------------------------
        //          Helpers
        // ------------------------- 

        // للحساب JWT عند إنشار حساب توليد   
        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var parsedUserId))
                throw new UnauthorizedAccessException("Invalid user id.");

            return parsedUserId;
        }
    }
}