using Tests.TokenSample.Model;

namespace Tests.TokenSample.Interfaces;

public interface ITokenService
{
    Task<(string token, int expiresInSeconds)> GenerateToken(User user);
}