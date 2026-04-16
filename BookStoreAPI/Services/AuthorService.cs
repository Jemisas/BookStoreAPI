using BookStoreAPI.Helpers;
using BookStoreAPI.Models;
using BookStoreAPI.Models.DTOs;
using BookStoreAPI.Repositories.Interfaces;
using BookStoreAPI.Services.Interfaces;

namespace BookStoreAPI.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<PaginatedResponseDto<AuthorResponseDto>> GetAllAsync(int page, int pageSize)
    {
        if (page < 1)
            throw new ArgumentException("page must be greater than or equal to 1.", nameof(page));
        if (pageSize < 1 || pageSize > 1000)
            throw new ArgumentException("pageSize must be between 1 and 1000.", nameof(pageSize));

        var (items, totalCount) = await _authorRepository.GetAllAsync(page, pageSize);

        return new PaginatedResponseDto<AuthorResponseDto>
        {
            Items = items.Select(MapToResponse).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<AuthorResponseDto?> GetByIdAsync(Guid id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        return author is null ? null : MapToResponse(author);
    }

    public async Task<AuthorResponseDto> CreateAsync(CreateAuthorDto dto)
    {
        var normalizedName = StringNormalizer.Normalize(dto.Name);
        if (string.IsNullOrWhiteSpace(normalizedName))
            throw new ArgumentException("Name becomes empty after normalization; provide a name with non-numeric characters.", nameof(dto.Name));

        var author = new Author
        {
            Id = Guid.NewGuid(),
            Name = normalizedName
        };

        var created = await _authorRepository.CreateAsync(author);
        return MapToResponse(created);
    }

    public async Task<AuthorResponseDto?> UpdateAsync(Guid id, UpdateAuthorDto dto)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if (author is null) return null;

        if (dto.Name is not null)
        {
            var normalizedName = StringNormalizer.Normalize(dto.Name);
            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new ArgumentException("Name becomes empty after normalization; provide a name with non-numeric characters.", nameof(dto.Name));
            author.Name = normalizedName;
        }

        var updated = await _authorRepository.UpdateAsync(author);
        return MapToResponse(updated);
    }

    public Task<bool> DeleteAsync(Guid id) => _authorRepository.DeleteAsync(id);

    private static AuthorResponseDto MapToResponse(Author author)
    {
        return new AuthorResponseDto
        {
            Id = author.Id,
            Name = author.Name,
            Books = author.Books?.Select(b => new BookResponseDto
            {
                Id = b.Id,
                Isbn = b.Isbn,
                Title = b.Title,
                CoverUrl = b.CoverUrl,
                PublicationYear = b.PublicationYear,
                AuthorId = b.AuthorId,
                AuthorName = author.Name
            }).ToList() ?? new List<BookResponseDto>()
        };
    }
}
