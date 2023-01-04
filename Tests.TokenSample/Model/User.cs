namespace Tests.TokenSample.Model;

public record User(Guid Id, string FullName, DateOnly BirthDate, string Document, string Email, string Password, string PwdSalt, Role[] Roles);