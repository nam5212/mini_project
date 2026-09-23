namespace BookManager.Tests.Repositories;

public class BookRepositoryTests : RepositoryTestBase
{
    private readonly BookRepository _repository;

    public BookRepositoryTests()
    {
        _repository = new BookRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddBookToDatabase_WhenCalled()
    {
        // Arrange
        var book = new Book
        {
            Title = "Clean Architecture",
            Author = "Robert Martin",
            Price = 45m,
            Category = "Software",
            Stock = 10
        };

        // Act
        var result = await _repository.AddAsync(book);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        var stored = await _context.Books.FindAsync(result.Id);
        stored.Should().NotBeNull();
        stored!.Title.Should().Be("Clean Architecture");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBook_WhenBookExists()
    {
        // Arrange
        var book = new Book { Title = "Refactoring", Author = "Martin Fowler", Price = 50m, Category = "Tech", Stock = 5 };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(book.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(book.Id);
        result.Title.Should().Be("Refactoring");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBookDoesNotExist()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterBySearchKeyword_WhenKeywordProvided()
    {
        // Arrange
        _context.Books.AddRange(
            new Book { Title = "C# in Depth", Author = "Jon Skeet", Price = 40m, Category = "Tech", Stock = 3 },
            new Book { Title = "Java Concurrency", Author = "Brian Goetz", Price = 45m, Category = "Tech", Stock = 4 },
            new Book { Title = "Fluent Python", Author = "Luciano Ramalho", Price = 50m, Category = "Python", Stock = 2 }
        );
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetAllAsync(search: "C#");

        // Assert
        items.Should().HaveCount(1);
        totalCount.Should().Be(1);
        items[0].Title.Should().Be("C# in Depth");
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterByPriceRange_WhenMinAndMaxPriceProvided()
    {
        // Arrange
        _context.Books.AddRange(
            new Book { Title = "Book 10", Author = "A", Price = 10m, Category = "Cat", Stock = 1 },
            new Book { Title = "Book 30", Author = "B", Price = 30m, Category = "Cat", Stock = 1 },
            new Book { Title = "Book 50", Author = "C", Price = 50m, Category = "Cat", Stock = 1 }
        );
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetAllAsync(minPrice: 20m, maxPrice: 40m);

        // Assert
        items.Should().HaveCount(1);
        totalCount.Should().Be(1);
        items[0].Title.Should().Be("Book 30");
    }

    [Fact]
    public async Task GetAllAsync_ShouldSortByPriceAscending_WhenSortIsPriceAsc()
    {
        // Arrange
        _context.Books.AddRange(
            new Book { Title = "Expensive", Author = "A", Price = 90m, Category = "Cat", Stock = 1 },
            new Book { Title = "Cheap", Author = "B", Price = 10m, Category = "Cat", Stock = 1 }
        );
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetAllAsync(sort: "price_asc");

        // Assert
        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
        items[0].Price.Should().Be(10m);
        items[1].Price.Should().Be(90m);
    }

    [Fact]
    public async Task GetAllAsync_ShouldSortByPriceDescending_WhenSortIsPriceDesc()
    {
        // Arrange
        _context.Books.AddRange(
            new Book { Title = "Cheap", Author = "B", Price = 10m, Category = "Cat", Stock = 1 },
            new Book { Title = "Expensive", Author = "A", Price = 90m, Category = "Cat", Stock = 1 }
        );
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _repository.GetAllAsync(sort: "price_desc");

        // Assert
        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
        items[0].Price.Should().Be(90m);
        items[1].Price.Should().Be(10m);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPaginatedResultsAndTotalCount_WhenPaginationProvided()
    {
        // Arrange
        for (int i = 1; i <= 15; i++)
        {
            _context.Books.Add(new Book { Title = $"Book {i:D2}", Author = "Author", Price = 10m * i, Category = "Cat", Stock = 1 });
        }
        await _context.SaveChangesAsync();

        // Act - Page 2 with PageSize 10
        var (items, totalCount) = await _repository.GetAllAsync(pageIndex: 2, pageSize: 10);

        // Assert
        items.Should().HaveCount(5);
        totalCount.Should().Be(15);
        items[0].Title.Should().Be("Book 11");
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges_WhenBookUpdated()
    {
        // Arrange
        var book = new Book { Title = "Original", Author = "Author", Price = 25m, Category = "Tech", Stock = 1 };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Act
        book.Title = "Updated Title";
        book.Price = 30m;
        await _repository.UpdateAsync(book);

        // Assert
        var updated = await _context.Books.FindAsync(book.Id);
        updated.Should().NotBeNull();
        updated!.Title.Should().Be("Updated Title");
        updated.Price.Should().Be(30m);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveBookFromDatabase_WhenBookDeleted()
    {
        // Arrange
        var book = new Book { Title = "To Delete", Author = "Author", Price = 25m, Category = "Tech", Stock = 1 };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(book);

        // Assert
        var deleted = await _context.Books.FindAsync(book.Id);
        deleted.Should().BeNull();
    }
}
