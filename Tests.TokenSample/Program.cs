using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Tests.TokenSample.Interfaces;
using Tests.TokenSample.Model;
using Tests.TokenSample.Repositories;
using Tests.TokenSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserRepository, MemoryUserRepository>();
builder.Services.AddScoped<IUserService, DumbUserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://www.token-sample.com/auth",
            ValidAudience = "token-sample"
        };
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddAuthorization();
builder.Services
    .AddJwksManager()
    .UseJwtValidation();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapPost("/api/auth", [AllowAnonymous] async (ITokenService tokenService, IUserService userService, Login login) =>
{
    var user = await userService.GetUserByEmailAndPassword(login.Email, login.Password);

    if (user is null)
        return Results.BadRequest(new {message = "User does not exists or the password was incorrect."});

    var (token, expiresInSeconds) = await tokenService.GenerateToken(user);

    return Results.Ok(new
    {
        token,
        expires_in = expiresInSeconds,
        user = new
        {
            user.Email,
            roles = user.Roles.Select(x => x.Name)
        }
    });
});

app.MapPost("/api/auth/anon", [AllowAnonymous]
    async (ITokenService tokenService, IUserService userService, AnonymousLogin anonymousLogin) =>
    {
        var (fullName, email, document) = anonymousLogin;
        var roles = new Role[] {new(Guid.NewGuid(), "anonymous")};

        var (token, expiresInSeconds) =
            await tokenService.GenerateTokenWithoutIdentification(fullName, email, document, roles);

        return Results.Ok(new
        {
            token,
            expires_in = expiresInSeconds,
            user = new
            {
                fullName,
                email,
                document,
                roles = roles.Select(x => x.Name)
            }
        });
    });

app.MapGet("/api/token/details", [Authorize](IHttpContextAccessor httpContextAccessor) =>
{
    return Results.Ok(new
    {
        claims = httpContextAccessor.HttpContext!.User.Claims.Select(x => new {x.Type, x.Value}),
        isAdmin = httpContextAccessor.HttpContext!.User.IsInRole("admin")
    });
});

app.MapPost("/api/admin", [Authorize(Roles = "admin")]() =>
    Results.Ok(new { message = "You're admin." }));

app.MapPost("/api/anonymous", [Authorize(Roles = "anonymous")]() =>
    Results.Ok(new { message = "You're anonymous." }));

app.MapPost("/api/accountant", [Authorize(Roles = "accountant")]() =>
    Results.Ok(new { message = "You're accountant." }));

app.MapPost("/api/developer", [Authorize(Roles = "developer")]() =>
    Results.Ok(new { message = "You're developer." }));

app.Run();