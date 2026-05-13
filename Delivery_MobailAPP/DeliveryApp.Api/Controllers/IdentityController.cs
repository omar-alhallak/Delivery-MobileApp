using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using DeliveryApp.Application.Features.Identity.Logout;
using DeliveryApp.Application.Features.Identity.LoginLocal;
using DeliveryApp.Application.Features.Identity.RefreshToken;
using DeliveryApp.Application.Features.Identity.RegisterLocal;
using DeliveryApp.Application.Features.Identity.UpdateMyProfile;

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

        public IdentityController(
            RegisterLocalService registerLocalService,
            LoginLocalService loginLocalService,
            UpdateMyProfileService updateMyProfileService,
            RefreshTokenService refreshTokenService,
            LogoutService logoutService)
        {
            _registerLocalService = registerLocalService;
            _loginLocalService = loginLocalService;
            _updateMyProfileService = updateMyProfileService;
            _refreshTokenService = refreshTokenService;
            _logoutService = logoutService;
        }

        [HttpPost("register/local")]
        public async Task<ActionResult<RegisterLocalResponse>> RegisterLocal(
            [FromBody] RegisterLocalRequest request,
            CancellationToken ct)
        {
            var response = await _registerLocalService.ExecuteAsync(request, ct);

            return Ok(response);
        }

        [HttpPost("login/local")]
        public async Task<ActionResult<LoginLocalResponse>> LoginLocal(
            [FromBody] LoginLocalRequest request,
            CancellationToken ct)
        {
            var response = await _loginLocalService.ExecuteAsync(request, ct);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = GetCurrentUserId();

            return Ok(new
            {
                UserId = userId
            });
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<ActionResult<UpdateMyProfileResponse>> UpdateMyProfile(
            [FromBody] UpdateMyProfileRequest request,
            CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _updateMyProfileService.ExecuteAsync(
                userId,
                request,
                ct);

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken(
            [FromBody] RefreshTokenRequest request,
            CancellationToken ct)
        {
            var response = await _refreshTokenService.ExecuteAsync(request, ct);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<LogoutResponse>> Logout(
            [FromBody] LogoutRequest request,
            CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _logoutService.ExecuteAsync(
                userId,
                request,
                ct);

            return Ok(response);
        }

        private Guid GetCurrentUserId()
        {
            var userId =
                User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var parsedUserId))
                throw new Exception("Invalid user id.");

            return parsedUserId;
        }
    }
}