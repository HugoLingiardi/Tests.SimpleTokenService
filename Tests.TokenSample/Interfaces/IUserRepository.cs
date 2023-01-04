using Tests.TokenSample.Model;

namespace Tests.TokenSample.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByEmail(string email);
}