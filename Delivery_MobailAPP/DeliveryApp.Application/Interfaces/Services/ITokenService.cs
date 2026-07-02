using DeliveryApp.Domain.Entities.Identity;

namespace DeliveryApp.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);

        string GenerateRefreshToken();

        byte[] HashRefreshToken(string refreshToken);

        TimeSpan GetRefreshTokenLifetime();
    }
}