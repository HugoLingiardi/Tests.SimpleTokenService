using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;
using Tests.TokenSample.Interfaces;
using Tests.TokenSample.Model;

namespace Tests.TokenSample.Services;

public class TokenService : ITokenService
{
    private readonly IJwtService _jwtService;

    public TokenService(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public async Task<(string token, int expiresInSeconds)> GenerateToken(User user)
    {
        var key = await _jwtService.GetCurrentSigningCredentials();
        var handler = new JsonWebTokenHandler();
        var now = DateTime.Now;

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.DateOfBirth, user.BirthDate.ToShortDateString()),
        };

        const int expiresInSeconds = 60 * 60;
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = "https://www.token-sample.com/auth",
            Audience = "token-sample",
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddSeconds(60),
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = key,
        };

        return (handler.CreateToken(descriptor), expiresInSeconds);
    }
}