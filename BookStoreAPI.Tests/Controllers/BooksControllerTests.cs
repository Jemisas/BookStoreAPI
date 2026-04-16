using System.Net;
using BookStoreAPI.Controllers;
using BookStoreAPI.Models.DTOs;
using BookStoreAPI.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

namespace BookStoreAPI.Tests.Controllers;

public class BooksControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BooksControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAll_ShouldReturn200_WithListOfBooks()
    {
        var paginated = new PaginatedResponseDto<BookResponseDto>
        {
            Items = new List<BookResponseDto>
            {
                new() { Id = Guid.NewGuid(), Isbn = "9780000000001", Title = "TEST BOOK", AuthorName = "JANE DOE" }
            },
            Page = 1,
            PageSize = 10,
            TotalCount = 1,
            TotalPages = 1
        };

        var service = new Mock<IBookService>();
        service.Setup(s => s.GetAllAsync(1, 10, null, null)).ReturnsAsync(paginated);

        var controller = new BooksController(service.Object);

        var result = await controller.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        var body = ok.Value.Should().BeOfType<PaginatedResponseDto<BookResponseDto>>().Subject;
        body.Items.Should().HaveCount(1);
        body.Items[0].Title.Should().Be("TEST BOOK");
    }

    [Fact]
    public async Task GetAll_WithoutJwtToken_ShouldReturn401()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/books");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
