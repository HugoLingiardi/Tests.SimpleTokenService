using Tests.TokenSample.Interfaces;
using Tests.TokenSample.Model;

namespace Tests.TokenSample.Repositories;

public class MemoryUserRepository : IUserRepository
{
    private readonly IList<User> _users;

    public MemoryUserRepository()
    {
        _users = new List<User>()
        {
            new(Guid.NewGuid(),
                "Higo Lungiardi",
                DateOnly.Parse("01/09/1990"),
                "05838749912",
                "hugo@hugo.com",
                "abcde",
                "salt",
                new Role[]{ new(Guid.NewGuid(), "admin")}),
            new(Guid.NewGuid(),
                "Clayton Poitevin",
                DateOnly.Parse("03/01/1991"),
                "05838749913",
                "claytin@clayton.com",
                "abcde",
                "salt",
                new Role[] { new(Guid.NewGuid(), "accountant"), new(Guid.NewGuid(), "developer")}),
        };
    }

    public Task<User?> GetUserByEmail(string email) => 
        Task.FromResult(_users.FirstOrDefault(x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)));
}