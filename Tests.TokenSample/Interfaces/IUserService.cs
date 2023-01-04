using Tests.TokenSample.Model;

namespace Tests.TokenSample.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByEmailAndPassword(string email, string pwd);
}