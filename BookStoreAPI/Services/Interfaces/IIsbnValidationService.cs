namespace BookStoreAPI.Services.Interfaces;

public interface IIsbnValidationService
{
    Task<bool> ValidateIsbnAsync(string isbn);
}
