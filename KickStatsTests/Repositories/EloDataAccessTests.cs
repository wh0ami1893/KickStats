using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KickStatsTests.Repositories;

[TestFixture]
public class EloDataAccessTests
{
    private ApplicationDbContext _dbContext;
    private EloDataAccess _eloDataAccess;

    [SetUp]
    public void SetUp()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabaseElo") // Unique database per test to ensure isolation
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _eloDataAccess = new EloDataAccess(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        // Delete database and dispose context after each test
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task GetEloByIdAsync_ShouldReturnElo_WhenEloExists()
    {
        // Arrange
        var elo = new Elo { Id = Guid.NewGuid(), Score = 1200, Player = new ApplicationUser { Id = Guid.NewGuid() } };
        _dbContext.Elos.Add(elo);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _eloDataAccess.GetEloByIdAsync(elo.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(elo.Id));
    }

    [Test]
    public void GetEloByIdAsync_ShouldThrowKeyNotFoundException_WhenEloDoesNotExist()
    {
        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await _eloDataAccess.GetEloByIdAsync(Guid.NewGuid());
        });
    }

    [Test]
    public async Task GetEloByPlayerIdAsync_ShouldReturnElo_WhenPlayerHasAnElo()
    {
        // Arrange
        var player = new ApplicationUser { Id = Guid.NewGuid() };
        var elo = new Elo { Id = Guid.NewGuid(), Score = 1300, Player = player };
        _dbContext.Elos.Add(elo);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _eloDataAccess.GetEloByPlayerIdAsync(player.Id);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Id, Is.EqualTo(elo.Id));
    }

    [Test]
    public void GetEloByPlayerIdAsync_ShouldThrowKeyNotFoundException_WhenPlayerHasNoElo()
    {
        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await _eloDataAccess.GetEloByPlayerIdAsync(Guid.NewGuid());
        });
    }

    [Test]
    public async Task AddEloAsync_ShouldAddEloToDatabase()
    {
        // Arrange
        var elo = new Elo { Id = Guid.NewGuid(), Score = 1100, Player = new ApplicationUser { Id = Guid.NewGuid() } };

        // Act
        await _eloDataAccess.AddEloAsync(elo);
        await _eloDataAccess.SaveChangesAsync();

        var result = await _dbContext.Elos.FirstOrDefaultAsync(e => e.Id == elo.Id);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Id, Is.EqualTo(elo.Id));
    }

    [Test]
    public void AddEloAsync_ShouldThrowArgumentNullException_WhenEloIsNull()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _eloDataAccess.AddEloAsync(null);
        });
    }

    [Test]
    public async Task UpdateEloAsync_ShouldUpdateEloInDatabase()
    {
        // Arrange
        var elo = new Elo { Id = Guid.NewGuid(), Score = 1400, Player = new ApplicationUser { Id = Guid.NewGuid() } };
        _dbContext.Elos.Add(elo);
        await _dbContext.SaveChangesAsync();

        // Modify Elo's Score
        elo.Score = 1500;

        // Act
        await _eloDataAccess.UpdateEloAsync(elo);
        await _eloDataAccess.SaveChangesAsync();

        var result = await _dbContext.Elos.FirstOrDefaultAsync(e => e.Id == elo.Id);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(1500, result.Score);
    }

    [Test]
    public void UpdateEloAsync_ShouldThrowArgumentNullException_WhenEloIsNull()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _eloDataAccess.UpdateEloAsync(null);
        });
    }

    [Test]
    public async Task DeleteEloAsync_ShouldDeleteEloFromDatabase()
    {
        // Arrange
        var elo = new Elo { Id = Guid.NewGuid(), Score = 1450, Player = new ApplicationUser { Id = Guid.NewGuid() } };
        _dbContext.Elos.Add(elo);
        await _dbContext.SaveChangesAsync();

        // Act
        await _eloDataAccess.DeleteEloAsync(elo.Id);
        await _eloDataAccess.SaveChangesAsync();

        var result = await _dbContext.Elos.FirstOrDefaultAsync(e => e.Id == elo.Id);

        // Assert
        Assert.Null(result);
    }

    [Test]
    public void DeleteEloAsync_ShouldThrowKeyNotFoundException_WhenEloDoesNotExist()
    {
        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await _eloDataAccess.DeleteEloAsync(Guid.NewGuid());
        });
    }
}