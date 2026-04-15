using System.Text.Json;
using BookStoreAPI.Services.Interfaces;

namespace BookStoreAPI.Services;

public class OpenLibraryService : IOpenLibraryService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenLibraryService> _logger;

    public OpenLibraryService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<OpenLibraryService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string?> GetCoverUrlAsync(string isbn)
    {
        try
        {
            var baseUrl = _configuration["ExternalServices:OpenLibrary:BaseUrl"]
                ?? "https://openlibrary.org/api/books";

            var client = _httpClientFactory.CreateClient("OpenLibrary");
            var url = $"{baseUrl}?bibkeys=ISBN:{isbn}&format=json";

            using var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var payload = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(payload);

            var key = $"ISBN:{isbn}";
            if (!doc.RootElement.TryGetProperty(key, out var bookElement))
                return null;

            if (!bookElement.TryGetProperty("thumbnail_url", out var thumbnailElement))
                return null;

            return thumbnailElement.GetString();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch cover URL from Open Library for ISBN {Isbn}", isbn);
            return null;
        }
    }
}
