namespace BookStoreAPI.Services.Interfaces;

public interface IOpenLibraryService
{
    Task<string?> GetCoverUrlAsync(string isbn);
}
