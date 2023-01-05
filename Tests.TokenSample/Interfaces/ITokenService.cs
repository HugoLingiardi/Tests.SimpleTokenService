using Tests.TokenSample.Model;

namespace Tests.TokenSample.Interfaces;

public interface ITokenService
{
    Task<(string token, int expireInSeconds)> GenerateToken(User user);
    Task<(string token, int expireInSeconds)> GenerateTokenWithoutIdentification(string fullName, string email, string document, Role[] roles);
}