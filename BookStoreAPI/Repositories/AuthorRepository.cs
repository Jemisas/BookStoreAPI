using BookStoreAPI.Data;
using BookStoreAPI.Models;
using BookStoreAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly AppDbContext _context;

    public AuthorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Author> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var query = _context.Authors.AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(a => a.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Author?> GetByIdAsync(Guid id)
    {
        return await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Author> CreateAsync(Author author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return author;
    }

    public async Task<Author> UpdateAsync(Author author)
    {
        _context.Authors.Update(author);
        await _context.SaveChangesAsync();
        return author;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author is null) return false;

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Author?> GetByNameAsync(string normalizedName)
    {
        return await _context.Authors
            .FirstOrDefaultAsync(a => a.Name == normalizedName);
    }
}
