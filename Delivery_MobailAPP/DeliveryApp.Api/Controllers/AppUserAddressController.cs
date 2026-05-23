using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DeliveryApp.Application.Features.Addresses;
using DeliveryApp.Domain.Entities.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;
using AddressID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.AddressTag>;

namespace DeliveryApp.API.Controllers;

[ApiController]
[Route("api/user-addresses")]
public class AppUserAddressController: ControllerBase
{
    private readonly AppUserAddressService _service;

    public AppUserAddressController(AppUserAddressService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet("my-addresses")]
    public async Task<ActionResult<IReadOnlyList<AddressResponse>>> GetMyAddressesAsync()
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                     User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        var request = new GetUserAddressRequest
        {
            UserID = UserID.From(parsedUserId)
        };

        return Ok(await _service.GetListAsync(request));
    }
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<AddressResponse>> CreateAsync([FromBody] CreateUserAddressRequest request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(userIdString, out var parsedUserId))
        {
            var userId = UserID.From(parsedUserId);

            var result = await _service.CreateAsync(userId, request);
            return Ok(result);
        }
        return Unauthorized();
    }
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AddressResponse>> GetByIdAsync(Guid id)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                     User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        var addressId = AddressID.From(id);

        var response = await _service.GetByIdAsync(addressId);

        if (response is null)
            return NotFound();

        return Ok(response);
    }

    [Authorize]
    [HttpPut("{id:guid}/set-default")]
    public async Task<IActionResult> SetAsDefaultAsync(Guid id, CancellationToken ct = default)
    {
        var userIdString = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                           User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out var parsedUserId))
            return Unauthorized();

        var userId = UserID.From(parsedUserId);
        var addressId = AddressID.From(id);

        await _service.SetAsDefaultAsync(userId, addressId, ct);

        return Ok();
    }

    [Authorize]
    [HttpPut("{id:guid}/update")]
    public async Task<ActionResult<AddressResponse>> UpdateAsync(Guid id, [FromBody] UpdateAddressRequest request, CancellationToken ct = default)
    {
        var userIdString = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                           User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out var parsedUserId))
            return Unauthorized();

        var userId = UserID.From(parsedUserId);
        var addressId = AddressID.From(id);

        var result = await _service.UpdateAsync(userId, addressId, request, ct);

        return Ok(result);
    }
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var userIdString = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                           User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out var parsedUserId))
            return Unauthorized();

        var userId = UserID.From(parsedUserId);
        var addressId = AddressID.From(id);

        await _service.DeleteAsync(userId, addressId, ct);


        return NoContent();
    }
}
