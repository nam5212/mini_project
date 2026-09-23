namespace BookManager.Tests.Repositories;

public class UserRepositoryTests : RepositoryTestBase
{
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldSaveUserToDatabase_WhenUserProvided()
    {
        // Arrange
        var user = new User
        {
            Username = "bob",
            PasswordHash = "hashed_pw",
            Role = "User"
        };

        // Act
        var result = await _repository.AddAsync(user);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        var stored = await _context.Users.FindAsync(result.Id);
        stored.Should().NotBeNull();
        stored!.Username.Should().Be("bob");
    }

    [Fact]
    public async Task UsernameExistsAsync_ShouldReturnTrue_WhenUsernameExists()
    {
        // Arrange
        var user = new User { Username = "existing_user", PasswordHash = "hash", Role = "User" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.UsernameExistsAsync("existing_user");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task UsernameExistsAsync_ShouldReturnFalse_WhenUsernameDoesNotExist()
    {
        // Act
        var exists = await _repository.UsernameExistsAsync("non_existent_user");

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Username = "alice", PasswordHash = "hash_alice", Role = "Admin" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUsernameAsync("alice");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("alice");
        result.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var result = await _repository.GetByUsernameAsync("ghost");

        // Assert
        result.Should().BeNull();
    }
}
