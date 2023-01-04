using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Tests.TokenSample.Interfaces;
using Tests.TokenSample.Model;
using Tests.TokenSample.Repositories;
using Tests.TokenSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserRepository, MemoryUserRepository>();
builder.Services.AddScoped<IUserService, DumbUserService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

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
builder.Services.AddAuthorization();
builder.Services
    .AddJwksManager()
    .UseJwtValidation();

var app = builder.Build();

app.MapPost("/api/auth", async (ITokenService tokenService, IUserService userService, Login login) =>
{
    var user = await userService.GetUserByEmailAndPassword(login.Email, login.Password);

    if (user is null) 
        return Results.BadRequest(new {message = "User does not exists or the password was incorrect."});

    var (token, expiresInSeconds) = await  tokenService.GenerateToken(user);
    
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

app.MapGet("/api/token/details", () => Results.Ok(new { })).RequireAuthorization();
    

app.Run();