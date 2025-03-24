using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace KickStatsTests.Repositories;

[TestFixture]
public class EloHistoryDataAccessTests
{
    private ApplicationDbContext _dbContext;
    private EloHistoryDataAccess _eloHistoryDataAccess;

    [SetUp]
    public void SetUp()
    {
        // Configure in-memory database with unique name for isolation
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabaseEloHistory")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _eloHistoryDataAccess = new EloHistoryDataAccess(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up after each test
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task GetEloHistoryByIdAsync_ShouldReturnEloHistory_WhenEloHistoryExists()
    {
        // Arrange
        var eloHistory = new EloHistory { Id = Guid.NewGuid(), PlayerId = Guid.NewGuid(), Score = 1400 };
        _dbContext.EloHistories.Add(eloHistory);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _eloHistoryDataAccess.GetEloHistoryByIdAsync(eloHistory.Id);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Id, Is.EqualTo(eloHistory.Id));
    }

    [Test]
    public void GetEloHistoryByIdAsync_ShouldThrowKeyNotFoundException_WhenEloHistoryDoesNotExist()
    {
        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await _eloHistoryDataAccess.GetEloHistoryByIdAsync(Guid.NewGuid());
        });
    }

    [Test]
    public async Task GetEloHistoryByPlayerIdAsync_WithLimitAndOffset_ShouldReturnLimitedEloHistories()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        for (int i = 0; i < 10; i++)
        {
            _dbContext.EloHistories.Add(new EloHistory { Id = Guid.NewGuid(), PlayerId = playerId, Score = 1400 + i });
        }

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _eloHistoryDataAccess.GetEloHistoryByPlayerIdAsync(playerId, 5, 2);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(5));
        Assert.IsTrue(result.All(eh => eh.PlayerId == playerId));
    }

    [Test]
    public async Task GetEloHistoryByPlayerIdAsync_WithLimit_ShouldReturnSpecifiedNumberOfEloHistories()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        for (int i = 0; i < 10; i++)
        {
            _dbContext.EloHistories.Add(new EloHistory { Id = Guid.NewGuid(), PlayerId = playerId, Score = 1400 + i });
        }

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _eloHistoryDataAccess.GetEloHistoryByPlayerIdAsync(playerId, 5);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(5));
        Assert.IsTrue(result.All(eh => eh.PlayerId == playerId));
    }

    [Test]
    public async Task GetEloHistoryByPlayerIdAsync_ShouldReturnAllEloHistoriesForPlayer()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        for (int i = 0; i < 10; i++)
        {
            _dbContext.EloHistories.Add(new EloHistory { Id = Guid.NewGuid(), PlayerId = playerId, Score = 1400 + i });
        }

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _eloHistoryDataAccess.GetEloHistoryByPlayerIdAsync(playerId);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(10));
        Assert.IsTrue(result.All(eh => eh.PlayerId == playerId));
    }

    [Test]
    public async Task GetEloHistoryByPlayerIdAsync_ShouldReturnEmpty_WhenPlayerHasNoEloHistories()
    {
        // Arrange
        var playerId = Guid.NewGuid();

        // Act
        var result = await _eloHistoryDataAccess.GetEloHistoryByPlayerIdAsync(playerId);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task AddEloHistoryAsync_ShouldAddEloHistoryToDatabase()
    {
        // Arrange
        var eloHistory = new EloHistory { Id = Guid.NewGuid(), PlayerId = Guid.NewGuid(), Score = 1500 };

        // Act
        await _eloHistoryDataAccess.AddEloHistoryAsync(eloHistory);
        await _eloHistoryDataAccess.SaveChangesAsync();

        var result = await _dbContext.EloHistories.FirstOrDefaultAsync(eh => eh.Id == eloHistory.Id);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Id, Is.EqualTo(eloHistory.Id));
    }

    [Test]
    public async Task DeleteEloHistoryAsync_ShouldRemoveEloHistoryFromDatabase()
    {
        // Arrange
        var eloHistory = new EloHistory { Id = Guid.NewGuid(), PlayerId = Guid.NewGuid(), Score = 1600 };
        _dbContext.EloHistories.Add(eloHistory);
        await _dbContext.SaveChangesAsync();

        // Act
        await _eloHistoryDataAccess.DeleteEloHistoryAsync(eloHistory.Id);
        await _eloHistoryDataAccess.SaveChangesAsync();

        var result = await _dbContext.EloHistories.FirstOrDefaultAsync(eh => eh.Id == eloHistory.Id);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void DeleteEloHistoryAsync_ShouldThrowKeyNotFoundException_WhenEloHistoryDoesNotExist()
    {
        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await _eloHistoryDataAccess.DeleteEloHistoryAsync(Guid.NewGuid());
        });
    }

    [Test]
    public async Task SaveChangesAsync_ShouldPersistChangesToDatabase()
    {
        // Arrange
        var eloHistory = new EloHistory { Id = Guid.NewGuid(), PlayerId = Guid.NewGuid(), Score = 1400 };

        // Act
        await _eloHistoryDataAccess.AddEloHistoryAsync(eloHistory);
        await _eloHistoryDataAccess.SaveChangesAsync();

        var result = await _dbContext.EloHistories.FirstOrDefaultAsync(eh => eh.Id == eloHistory.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(eloHistory.Id));
    }
}