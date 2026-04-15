using BookStoreAPI.IsbnService;
using BookStoreAPI.Services.Interfaces;

namespace BookStoreAPI.Services;

public class IsbnValidationService : IIsbnValidationService
{
    private readonly ILogger<IsbnValidationService> _logger;

    public IsbnValidationService(ILogger<IsbnValidationService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> ValidateIsbnAsync(string isbn)
    {
        try
        {
            using var client = new SBNServiceSoapTypeClient(SBNServiceSoapTypeClient.EndpointConfiguration.ISBNServiceSoap);
            var response = await client.IsValidISBN13Async(isbn);
            return response.Body.IsValidISBN13Result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SOAP ISBN validation failed for {Isbn}; defaulting to valid", isbn);
            return true;
        }
    }
}
