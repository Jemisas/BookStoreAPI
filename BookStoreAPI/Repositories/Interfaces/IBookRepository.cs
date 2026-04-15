using BookStoreAPI.Models;

namespace BookStoreAPI.Repositories.Interfaces;

public interface IBookRepository
{
    Task<(List<Book> Items, int TotalCount)> GetAllAsync(int page, int pageSize, string? titleFilter, string? authorFilter);
    Task<Book?> GetByIdAsync(Guid id);
    Task<Book> CreateAsync(Book book);
    Task<Book> UpdateAsync(Book book);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsByIsbnAsync(string isbn);
}
