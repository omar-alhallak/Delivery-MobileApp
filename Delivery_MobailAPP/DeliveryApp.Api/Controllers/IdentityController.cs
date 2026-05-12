using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using DeliveryApp.Application.Features.Identity.LoginLocal;
using DeliveryApp.Application.Features.Identity.RegisterLocal;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Route("api/identity")]
    public sealed class IdentityController : ControllerBase
    {
        private readonly RegisterLocalService _registerLocalService;
        private readonly LoginLocalService _loginLocalService;

        public IdentityController(
            RegisterLocalService registerLocalService,
            LoginLocalService loginLocalService)
        {
            _registerLocalService = registerLocalService;
            _loginLocalService = loginLocalService;
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
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) 
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(new
            {
                UserId = userId
            });
        }
    }
}