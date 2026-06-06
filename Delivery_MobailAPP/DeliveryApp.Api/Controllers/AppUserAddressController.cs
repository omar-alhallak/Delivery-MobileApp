using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DeliveryApp.Application.Features.Addresses;
using DeliveryApp.Domain.Entities.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;
using AddressID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.AddressTag>;
using DeliveryApp.Application.Features.Addresses.CreateAddress;
using DeliveryApp.Application.Features.Addresses.UpdateAddress;
using DeliveryApp.Application.Features.Addresses.DeleteAddress;
using DeliveryApp.Application.Features.Addresses.GetUserAddress;

namespace DeliveryApp.API.Controllers;

[ApiController]
[Route("api/user-addresses")]
public class AppUserAddressController: ControllerBase
{
    private readonly CreateAddressService _createaddressservice;
    private readonly UpdateAddressService _updateaddressservice;
    private readonly DeleteAddressService _deleteaddressservice;
    private readonly GetAddressService _getaddressservice;


    public AppUserAddressController(CreateAddressService createaddressservice, UpdateAddressService updateaddressservice, DeleteAddressService deleteaddressservice, GetAddressService getaddressservice)
    {
        _createaddressservice = createaddressservice;
        _updateaddressservice = updateaddressservice;
        _deleteaddressservice = deleteaddressservice;
        _getaddressservice = getaddressservice;
    }

    [Authorize]
    [HttpGet("my-addresses")]
    public async Task<ActionResult<IReadOnlyList<GetAddressResponse>>> GetMyAddressesAsync()
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                     User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        var request = new GetUserAddressRequest
        {
            UserID = UserID.From(parsedUserId)
        };

        return Ok(await _getaddressservice.GetListAsync(request));
    }
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<CreateAddressResponse>> CreateAsync([FromBody] CreateUserAddressRequest request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(userIdString, out var parsedUserId))
        {
            var userId = UserID.From(parsedUserId);

            var result = await _createaddressservice.CreateAsync(userId, request);
            return Ok(result);
        }
        return Unauthorized();
    }
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetAddressResponse>> GetByIdAsync(Guid id)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                     User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var parsedUserId))
            return Unauthorized();

        var addressId = AddressID.From(id);

        var response = await _getaddressservice.GetByIdAsync(addressId);

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

        await _getaddressservice.SetAsDefaultAsync(userId, addressId, ct);

        return Ok();
    }

    [Authorize]
    [HttpPut("{id:guid}/update")]
    public async Task<ActionResult<UpdateAddressResponse>> UpdateAsync(Guid id, [FromBody] UpdateAddressRequest request, CancellationToken ct = default)
    {
        var userIdString = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                           User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out var parsedUserId))
            return Unauthorized();

        var userId = UserID.From(parsedUserId);
        var addressId = AddressID.From(id);

        var result = await _updateaddressservice.UpdateAsync(userId, addressId, request, ct);

        return Ok(result);
    }
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> _DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var userIdString = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                           User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out var parsedUserId))
            return Unauthorized();

        var userId = UserID.From(parsedUserId);
        var addressId = AddressID.From(id);

        await _deleteaddressservice.DeleteAsync(userId, addressId, ct);


        return NoContent();
    }
}
