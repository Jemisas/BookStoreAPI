using BookStoreAPI.Models;

namespace BookStoreAPI.Repositories.Interfaces;

public interface IAuthorRepository
{
    Task<(List<Author> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
    Task<Author?> GetByIdAsync(Guid id);
    Task<Author> CreateAsync(Author author);
    Task<Author> UpdateAsync(Author author);
    Task<bool> DeleteAsync(Guid id);
    Task<Author?> GetByNameAsync(string normalizedName);
}
