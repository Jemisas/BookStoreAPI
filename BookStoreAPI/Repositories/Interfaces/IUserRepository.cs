using BookStoreAPI.Models;

namespace BookStoreAPI.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
}
