using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using DeliveryApp.Application.Features.Merchants.GetMerchants;
using DeliveryApp.Application.Features.Merchants.CreateMerchant;
using DeliveryApp.Application.Features.Merchants.UpdateMerchant;
using DeliveryApp.Application.Features.Merchants.SetWorkingHours;
using DeliveryApp.Application.Features.Merchants.GetWorkingHours;
using DeliveryApp.Application.Features.Merchants.ActivateMerchant;
using DeliveryApp.Application.Features.Merchants.AddMerchantStaff;
using DeliveryApp.Application.Features.Merchants.GetMerchantStaff;
using DeliveryApp.Application.Features.Merchants.RemoveMerchantStaff;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Route("api/merchant")]
    public sealed class MerchantController : ControllerBase
    {
        private readonly CreateMerchantService _createMerchantService;
        private readonly UpdateMerchantService _updateMerchantService;
        private readonly SetMerchantWorkingHoursService _setMerchantWorkingHoursService;
        private readonly ActivateMerchantService _activateMerchantService;
        private readonly GetMerchantsService _getMerchantsService;
        private readonly AddMerchantStaffService _addMerchantStaffService;
        private readonly RemoveMerchantStaffService _removeMerchantStaffService;
        private readonly GetMerchantStaffService _getMerchantStaffService;
        private readonly GetMerchantWorkingHoursService _getMerchantWorkingHoursService;

        public MerchantController(
            CreateMerchantService createMerchantService, UpdateMerchantService updateMerchantService, SetMerchantWorkingHoursService setMerchantWorkingHoursService,
            ActivateMerchantService activateMerchantService, GetMerchantsService getMerchantsService, AddMerchantStaffService addMerchantStaffService,
            RemoveMerchantStaffService removeMerchantStaffService, GetMerchantStaffService getMerchantStaffService, GetMerchantWorkingHoursService getMerchantWorkingHoursService)
        {
            _createMerchantService = createMerchantService ?? throw new ArgumentNullException(nameof(createMerchantService));

            _updateMerchantService = updateMerchantService ?? throw new ArgumentNullException(nameof(updateMerchantService));

            _setMerchantWorkingHoursService = setMerchantWorkingHoursService ?? throw new ArgumentNullException(nameof(setMerchantWorkingHoursService));

            _activateMerchantService = activateMerchantService ?? throw new ArgumentNullException(nameof(activateMerchantService));

            _getMerchantsService = getMerchantsService ?? throw new ArgumentNullException(nameof(getMerchantsService));

            _addMerchantStaffService = addMerchantStaffService ?? throw new ArgumentNullException(nameof(addMerchantStaffService));

            _removeMerchantStaffService = removeMerchantStaffService ?? throw new ArgumentNullException(nameof(removeMerchantStaffService));

            _getMerchantStaffService = getMerchantStaffService ?? throw new ArgumentNullException(nameof(getMerchantStaffService));

            _getMerchantWorkingHoursService = getMerchantWorkingHoursService ?? throw new ArgumentNullException(nameof(getMerchantWorkingHoursService));
        }

        [Authorize]
        [HttpPost("register")]
        public async Task<ActionResult<CreateMerchantResponse>> CreateMerchant([FromBody] CreateMerchantRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _createMerchantService.ExecuteAsync(userId, request, ct);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult<UpdateMerchantResponse>> UpdateMerchant([FromBody] UpdateMerchantRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _updateMerchantService.ExecuteAsync(userId, request, ct);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("working-hours")]
        public async Task<ActionResult<SetMerchantWorkingHoursResponse>> SetWorkingHours([FromBody] SetMerchantWorkingHoursRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _setMerchantWorkingHoursService.ExecuteAsync(userId, request, ct);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("activate")]
        public async Task<ActionResult<ActivateMerchantResponse>> ActivateMerchant([FromBody] ActivateMerchantRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _activateMerchantService.ExecuteAsync(userId, request, ct);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<GetMerchantsResponse>>> GetMerchants([FromQuery] GetMerchantsRequest request, CancellationToken ct)
        {
            var response = await _getMerchantsService.ExecuteAsync(request, ct);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("staff/add")]
        public async Task<ActionResult<AddMerchantStaffResponse>> AddStaff([FromBody] AddMerchantStaffRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _addMerchantStaffService.ExecuteAsync(userId, request, ct);

            return Ok(response);
        }

        [Authorize]
        [HttpDelete("staff/remove")]
        public async Task<ActionResult<RemoveMerchantStaffResponse>> RemoveStaff([FromBody] RemoveMerchantStaffRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _removeMerchantStaffService.ExecuteAsync(userId, request, ct);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("staff")]
        public async Task<ActionResult<GetMerchantStaffResponse>> GetStaff([FromQuery] GetMerchantStaffRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _getMerchantStaffService.ExecuteAsync(userId, request, ct);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("working-hours")]
        public async Task<ActionResult<GetMerchantWorkingHoursResponse>> GetWorkingHours([FromQuery] GetMerchantWorkingHoursRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var response = await _getMerchantWorkingHoursService.ExecuteAsync(userId, request, ct);

            return Ok(response);
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var parsedUserId))
                throw new UnauthorizedAccessException("Invalid user Id.");

            return parsedUserId;
        }
    }
}