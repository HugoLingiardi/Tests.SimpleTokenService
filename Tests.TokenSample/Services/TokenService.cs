using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;
using Tests.TokenSample.Interfaces;
using Tests.TokenSample.Model;

namespace Tests.TokenSample.Services;

public class TokenService : ITokenService
{
    private const int ExpireInSeconds = 3_600;
    private const string AccessTokenType = "at+jwt";
    private const string RefreshTokenType = "rt+jwt";
    
    private readonly IJwtService _jwtService;

    public TokenService(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public Task<(string token, int expireInSeconds)> GenerateToken(User user)
    {
        var rolesAsClaims = user.Roles.Select(x => new Claim("role", x.Name));
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new(JwtRegisteredClaimNames.Birthdate, user.BirthDate.ToShortDateString()),
            new(JwtRegisteredClaimNames.Sub, $"{user.Id}"),
        }.Concat(rolesAsClaims);

        return GenerateToken(claims);
    }

    public Task<(string token, int expireInSeconds)> GenerateTokenWithoutIdentification(string fullName, string email, string document, Role[] roles)
    {
        return GenerateToken(new Claim[]
        {
            new(JwtRegisteredClaimNames.Name, fullName),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Sub, document),
        }.Concat(roles.Select(x => new Claim("role", x.Name))));
    }

    private async Task<(string token, int expireInSeconds)> GenerateToken(IEnumerable<Claim> claims, string tokenType = AccessTokenType)
    {
        var key = await _jwtService.GetCurrentSigningCredentials();
        var handler = new JsonWebTokenHandler();
        var now = DateTime.Now;

        const int expireInSeconds = 60 * 60;
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = "https://www.token-sample.com/auth",
            Audience = "token-sample",
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddSeconds(60),
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = key,
            TokenType = tokenType
        };

        return (handler.CreateToken(descriptor), expireInSeconds);
    }
}