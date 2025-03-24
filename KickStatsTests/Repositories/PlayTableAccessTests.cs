using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace KickStatsTests.Repositories;

[TestFixture]
public class PlayTableDataAccessTests
{
    private ApplicationDbContext _dbContext;
    private PlayTableDataAccess _playTableDataAccess;

    [SetUp]
    public void SetUp()
    {
        // Configure an in-memory database with a unique name for test isolation
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabasePlayTables")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _playTableDataAccess = new PlayTableDataAccess(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up the database after each test
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnPlayTable_WhenPlayTableExists()
    {
        // Arrange
        var playTable = new PlayTable
        {
            Id = Guid.NewGuid(),
            Name = "Table1",
            Matches = new List<Match>
            {
                new Match { Id = Guid.NewGuid()},
                new Match { Id = Guid.NewGuid()}
            }
        };
        _dbContext.PlayTables.Add(playTable);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _playTableDataAccess.GetByIdAsync(playTable.Id);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Id, Is.EqualTo(playTable.Id));
        Assert.That(result.Matches.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNull_WhenPlayTableDoesNotExist()
    {
        // Act
        var result = await _playTableDataAccess.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Test]
    public async Task AddAsync_ShouldAddPlayTableToDatabase()
    {
        // Arrange
        var playTable = new PlayTable { Id = Guid.NewGuid(), Name = "Table2" };

        // Act
        await _playTableDataAccess.AddAsync(playTable);
        await _playTableDataAccess.SaveChangesAsync();

        var result = await _dbContext.PlayTables.FindAsync(playTable.Id);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Id, Is.EqualTo(playTable.Id));
    }

    [Test]
    public void AddAsync_ShouldThrowArgumentNullException_WhenPlayTableIsNull()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _playTableDataAccess.AddAsync(null);
        });
    }

    [Test]
    public async Task UpdateAsync_ShouldModifyPlayTableInDatabase()
    {
        // Arrange
        var existingTable = new PlayTable { Id = Guid.NewGuid(), Name = "Table3" };
        _dbContext.PlayTables.Add(existingTable);
        await _dbContext.SaveChangesAsync();

        // Update the table's name
        existingTable.Name = "Updated Table3";

        // Act
        await _playTableDataAccess.UpdateAsync(existingTable);
        await _playTableDataAccess.SaveChangesAsync();

        var result = await _dbContext.PlayTables.FindAsync(existingTable.Id);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Name, Is.EqualTo("Updated Table3"));
    }

    [Test]
    public void UpdateAsync_ShouldThrowArgumentNullException_WhenPlayTableIsNull()
    {
        // Act & Assert
        Assert.ThrowsAsync<NullReferenceException>(async () =>
        {
            await _playTableDataAccess.UpdateAsync(null);
        });
    }

    [Test]
    public async Task GetMatchesForPlayTableAsync_ShouldReturnMatches_WhenMatchesExist()
    {
        // Arrange
        var playTableId = Guid.NewGuid();
        var matches = new List<Match>
        {
            new Match { Id = Guid.NewGuid(), PlayTableId = playTableId },
            new Match { Id = Guid.NewGuid(), PlayTableId = playTableId}
        };
        _dbContext.Matches.AddRange(matches);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _playTableDataAccess.GetMatchesForPlayTableAsync(playTableId);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.IsTrue(result.All(m => m.PlayTableId == playTableId));
    }

    [Test]
    public async Task GetMatchesForPlayTableAsync_ShouldReturnEmpty_WhenNoMatchesExistForPlayTable()
    {
        // Arrange
        var playTableId = Guid.NewGuid();

        // Act
        var result = await _playTableDataAccess.GetMatchesForPlayTableAsync(playTableId);

        // Assert
        Assert.IsEmpty(result);
    }
}