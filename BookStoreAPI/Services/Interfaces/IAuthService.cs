using BookStoreAPI.Models.DTOs;

namespace BookStoreAPI.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
}
