using System.Globalization;
using BookStoreAPI.Helpers;
using BookStoreAPI.Models;
using BookStoreAPI.Models.DTOs;
using BookStoreAPI.Repositories.Interfaces;
using BookStoreAPI.Services.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;

namespace BookStoreAPI.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IIsbnValidationService _isbnValidationService;
    private readonly IOpenLibraryService _openLibraryService;

    public BookService(
        IBookRepository bookRepository,
        IAuthorRepository authorRepository,
        IIsbnValidationService isbnValidationService,
        IOpenLibraryService openLibraryService)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _isbnValidationService = isbnValidationService;
        _openLibraryService = openLibraryService;
    }

    public async Task<PaginatedResponseDto<BookResponseDto>> GetAllAsync(int page, int pageSize, string? titleFilter, string? authorFilter)
    {
        if (page < 1)
            throw new ArgumentException("page must be greater than or equal to 1.", nameof(page));
        if (pageSize < 1 || pageSize > 1000)
            throw new ArgumentException("pageSize must be between 1 and 1000.", nameof(pageSize));

        var (items, totalCount) = await _bookRepository.GetAllAsync(page, pageSize, titleFilter, authorFilter);

        return new PaginatedResponseDto<BookResponseDto>
        {
            Items = items.Select(MapToResponse).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<BookResponseDto?> GetByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book is null ? null : MapToResponse(book);
    }

    public async Task<BookResponseDto> CreateAsync(CreateBookDto dto)
    {
        var isValidIsbn = await _isbnValidationService.ValidateIsbnAsync(dto.Isbn);
        if (!isValidIsbn)
            throw new ArgumentException($"The ISBN '{dto.Isbn}' is invalid.", nameof(dto.Isbn));

        if (await _bookRepository.ExistsByIsbnAsync(dto.Isbn))
            throw new ArgumentException($"A book with ISBN '{dto.Isbn}' already exists.", nameof(dto.Isbn));

        var author = await _authorRepository.GetByIdAsync(dto.AuthorId)
            ?? throw new ArgumentException($"Author with id '{dto.AuthorId}' does not exist.", nameof(dto.AuthorId));

        var normalizedTitle = StringNormalizer.Normalize(dto.Title);
        if (string.IsNullOrWhiteSpace(normalizedTitle))
            throw new ArgumentException("Title becomes empty after normalization; provide a title with non-numeric characters.", nameof(dto.Title));

        var coverUrl = await _openLibraryService.GetCoverUrlAsync(dto.Isbn);

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Isbn = dto.Isbn,
            Title = normalizedTitle,
            CoverUrl = coverUrl,
            PublicationYear = dto.PublicationYear,
            AuthorId = author.Id
        };

        var created = await _bookRepository.CreateAsync(book);
        return MapToResponse(created);
    }

    public async Task<BookResponseDto?> UpdateAsync(Guid id, UpdateBookDto dto)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null) return null;

        if (dto.Title is not null)
        {
            var normalizedTitle = StringNormalizer.Normalize(dto.Title);
            if (string.IsNullOrWhiteSpace(normalizedTitle))
                throw new ArgumentException("Title becomes empty after normalization; provide a title with non-numeric characters.", nameof(dto.Title));
            book.Title = normalizedTitle;
        }

        if (dto.PublicationYear.HasValue)
            book.PublicationYear = dto.PublicationYear.Value;

        if (dto.AuthorId.HasValue)
        {
            var newAuthor = await _authorRepository.GetByIdAsync(dto.AuthorId.Value)
                ?? throw new ArgumentException($"Author with id '{dto.AuthorId}' does not exist.", nameof(dto.AuthorId));
            book.AuthorId = newAuthor.Id;
        }

        var updated = await _bookRepository.UpdateAsync(book);
        return MapToResponse(updated);
    }

    public Task<bool> DeleteAsync(Guid id) => _bookRepository.DeleteAsync(id);

    public Task<bool> ValidateIsbnAsync(string isbn) => _isbnValidationService.ValidateIsbnAsync(isbn);

    public async Task<List<BookResponseDto>> CreateMassiveAsync(Stream csvStream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            PrepareHeaderForMatch = args => args.Header.Trim().ToLowerInvariant()
        };

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, config);

        var records = csv.GetRecords<CsvBookRecord>().ToList();
        var created = new List<BookResponseDto>();

        foreach (var record in records)
        {
            if (string.IsNullOrWhiteSpace(record.Isbn) || string.IsNullOrWhiteSpace(record.AuthorName))
                continue;

            var normalizedTitle = StringNormalizer.Normalize(record.Title);
            if (string.IsNullOrWhiteSpace(normalizedTitle))
                continue;

            var normalizedAuthorName = StringNormalizer.Normalize(record.AuthorName);
            if (string.IsNullOrWhiteSpace(normalizedAuthorName))
                continue;

            if (await _bookRepository.ExistsByIsbnAsync(record.Isbn))
                continue;

            var isValidIsbn = await _isbnValidationService.ValidateIsbnAsync(record.Isbn);
            if (!isValidIsbn)
                continue;

            var author = await _authorRepository.GetByNameAsync(normalizedAuthorName);
            if (author is null)
            {
                author = await _authorRepository.CreateAsync(new Author
                {
                    Id = Guid.NewGuid(),
                    Name = normalizedAuthorName
                });
            }

            var coverUrl = await _openLibraryService.GetCoverUrlAsync(record.Isbn);

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Isbn = record.Isbn,
                Title = normalizedTitle,
                CoverUrl = coverUrl,
                PublicationYear = record.PublicationYear,
                AuthorId = author.Id
            };

            var persisted = await _bookRepository.CreateAsync(book);
            created.Add(MapToResponse(persisted));
        }

        return created;
    }

    private static BookResponseDto MapToResponse(Book book)
    {
        return new BookResponseDto
        {
            Id = book.Id,
            Isbn = book.Isbn,
            Title = book.Title,
            CoverUrl = book.CoverUrl,
            PublicationYear = book.PublicationYear,
            AuthorId = book.AuthorId,
            AuthorName = book.Author?.Name ?? string.Empty
        };
    }
}
