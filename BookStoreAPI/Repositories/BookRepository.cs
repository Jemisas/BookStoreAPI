using BookStoreAPI.Data;
using BookStoreAPI.Models;
using BookStoreAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Book> Items, int TotalCount)> GetAllAsync(int page, int pageSize, string? titleFilter, string? authorFilter)
    {
        var query = _context.Books
            .Include(b => b.Author)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(titleFilter))
        {
            var term = titleFilter.ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(authorFilter))
        {
            var term = authorFilter.ToLower();
            query = query.Where(b => b.Author.Name.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        return await _context.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Book> CreateAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        await _context.Entry(book).Reference(b => b.Author).LoadAsync();
        return book;
    }

    public async Task<Book> UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
        await _context.Entry(book).Reference(b => b.Author).LoadAsync();
        return book;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByIsbnAsync(string isbn)
    {
        return await _context.Books.AnyAsync(b => b.Isbn == isbn);
    }
}
