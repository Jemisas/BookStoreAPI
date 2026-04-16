using BookStoreAPI.Models;
using BookStoreAPI.Models.DTOs;
using BookStoreAPI.Repositories.Interfaces;
using BookStoreAPI.Services;
using BookStoreAPI.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStoreAPI.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepo = new();
    private readonly Mock<IAuthorRepository> _authorRepo = new();
    private readonly Mock<IIsbnValidationService> _isbnService = new();
    private readonly Mock<IOpenLibraryService> _openLibraryService = new();

    private BookService CreateSut() => new(
        _bookRepo.Object,
        _authorRepo.Object,
        _isbnService.Object,
        _openLibraryService.Object);

    [Fact]
    public async Task CreateAsync_ShouldNormalizeTitle()
    {
        var authorId = Guid.NewGuid();
        var author = new Author { Id = authorId, Name = "JANE DOE" };

        _isbnService.Setup(s => s.ValidateIsbnAsync(It.IsAny<string>())).ReturnsAsync(true);
        _bookRepo.Setup(r => r.ExistsByIsbnAsync(It.IsAny<string>())).ReturnsAsync(false);
        _authorRepo.Setup(r => r.GetByIdAsync(authorId)).ReturnsAsync(author);
        _openLibraryService.Setup(s => s.GetCoverUrlAsync(It.IsAny<string>())).ReturnsAsync("https://covers.fake/img.jpg");
        _bookRepo.Setup(r => r.CreateAsync(It.IsAny<Book>()))
            .ReturnsAsync((Book b) =>
            {
                b.Author = author;
                return b;
            });

        var dto = new CreateBookDto
        {
            Isbn = "9780000000001",
            Title = "  café 123  lámpara  ",
            PublicationYear = 2020,
            AuthorId = authorId
        };

        var sut = CreateSut();
        var result = await sut.CreateAsync(dto);

        result.Title.Should().Be("CAFE LAMPARA");
        _bookRepo.Verify(r => r.CreateAsync(It.Is<Book>(b => b.Title == "CAFE LAMPARA")), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenIsbnIsInvalid()
    {
        _isbnService.Setup(s => s.ValidateIsbnAsync(It.IsAny<string>())).ReturnsAsync(false);

        var dto = new CreateBookDto
        {
            Isbn = "bad-isbn",
            Title = "Anything",
            PublicationYear = 2020,
            AuthorId = Guid.NewGuid()
        };

        var sut = CreateSut();
        var act = () => sut.CreateAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>();
        _bookRepo.Verify(r => r.CreateAsync(It.IsAny<Book>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenIsbnAlreadyExists()
    {
        _isbnService.Setup(s => s.ValidateIsbnAsync(It.IsAny<string>())).ReturnsAsync(true);
        _bookRepo.Setup(r => r.ExistsByIsbnAsync(It.IsAny<string>())).ReturnsAsync(true);

        var dto = new CreateBookDto
        {
            Isbn = "9780000000001",
            Title = "Anything",
            PublicationYear = 2020,
            AuthorId = Guid.NewGuid()
        };

        var sut = CreateSut();
        var act = () => sut.CreateAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>();
        _bookRepo.Verify(r => r.CreateAsync(It.IsAny<Book>()), Times.Never);
    }
}
