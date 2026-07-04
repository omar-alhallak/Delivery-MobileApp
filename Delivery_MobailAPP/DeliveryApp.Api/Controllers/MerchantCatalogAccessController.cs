using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DeliveryApp.Application.Features.MerchantCatalog.Access;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/merchant")]
    public sealed class MerchantCatalogAccessController : ControllerBase // يعيد المطاعم التي يستطيع المستخدم إدارة كتالوجها
    {
        private readonly MerchantCatalogAccessService _accessService;

        public MerchantCatalogAccessController(MerchantCatalogAccessService accessService)
        {
            _accessService = accessService;
        }

        [HttpGet("my-merchants")]
        public async Task<ActionResult<IReadOnlyList<MyMerchantDto>>> GetMyMerchants(CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            return Ok(await _accessService.GetMyMerchantsAsync(userId, ct));
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
