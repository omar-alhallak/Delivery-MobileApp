using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using DeliveryApp.Application.Features.Merchants.CreateMerchant;
using DeliveryApp.Application.Features.Merchants.UpdateMerchant;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/merchant")]
    public sealed class MerchantController : ControllerBase
    {
        private readonly CreateMerchantService _createMerchantService;
        private readonly UpdateMerchantService _updateMerchantService;

        public MerchantController(CreateMerchantService createMerchantService, UpdateMerchantService updateMerchantService)
        {
            _createMerchantService = createMerchantService ?? throw new ArgumentNullException(nameof(createMerchantService));

            _updateMerchantService = updateMerchantService ?? throw new ArgumentNullException(nameof(updateMerchantService));
        }

        [HttpPost]
        public async Task<ActionResult<CreateMerchantResponse>> CreateMerchant([FromBody] CreateMerchantRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _createMerchantService.ExecuteAsync(userId, request, ct);

            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<UpdateMerchantResponse>> UpdateMerchant([FromBody] UpdateMerchantRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _updateMerchantService.ExecuteAsync(userId, request, ct);

            return Ok(response);
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var parsedUserId))
                throw new UnauthorizedAccessException("Invalid user id.");

            return parsedUserId;
        }
    }
}