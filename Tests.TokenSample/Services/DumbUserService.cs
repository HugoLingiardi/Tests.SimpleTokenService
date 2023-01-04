using Tests.TokenSample.Interfaces;
using Tests.TokenSample.Model;

namespace Tests.TokenSample.Services;

public class DumbUserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public DumbUserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetUserByEmailAndPassword(string email, string pwd)
    {
        var user = await _userRepository.GetUserByEmail(email);

        if (user is null || user is {Password: var storedPwd} && storedPwd != pwd)
        {
            return null;
        }

        return user;
    }
}