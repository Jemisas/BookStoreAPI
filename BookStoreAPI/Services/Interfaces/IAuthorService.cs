using BookStoreAPI.Models.DTOs;

namespace BookStoreAPI.Services.Interfaces;

public interface IAuthorService
{
    Task<PaginatedResponseDto<AuthorResponseDto>> GetAllAsync(int page, int pageSize);
    Task<AuthorResponseDto?> GetByIdAsync(Guid id);
    Task<AuthorResponseDto> CreateAsync(CreateAuthorDto dto);
    Task<AuthorResponseDto?> UpdateAsync(Guid id, UpdateAuthorDto dto);
    Task<bool> DeleteAsync(Guid id);
}
