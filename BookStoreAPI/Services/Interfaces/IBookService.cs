using BookStoreAPI.Models.DTOs;

namespace BookStoreAPI.Services.Interfaces;

public interface IBookService
{
    Task<PaginatedResponseDto<BookResponseDto>> GetAllAsync(int page, int pageSize, string? titleFilter, string? authorFilter);
    Task<BookResponseDto?> GetByIdAsync(Guid id);
    Task<BookResponseDto> CreateAsync(CreateBookDto dto);
    Task<BookResponseDto?> UpdateAsync(Guid id, UpdateBookDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ValidateIsbnAsync(string isbn);
    Task<List<BookResponseDto>> CreateMassiveAsync(Stream csvStream);
}
