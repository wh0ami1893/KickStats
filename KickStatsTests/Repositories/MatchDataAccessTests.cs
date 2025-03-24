using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace KickStatsTests.Repositories;

[TestFixture]
public class MatchDataAccessTests
{
    private ApplicationDbContext _dbContext;
    private MatchDataAccess _matchDataAccess;

    [SetUp]
    public void SetUp()
    {
        // Configure an in-memory database with a unique name for isolation
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _matchDataAccess = new MatchDataAccess(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up the database after each test
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnMatch_WhenMatchExists()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var match = new Match
        {
            Id = matchId
        };
        _dbContext.Matches.Add(match);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _matchDataAccess.GetByIdAsync(matchId);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Id, Is.EqualTo(matchId));
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNull_WhenMatchDoesNotExist()
    {
        // Act
        var result = await _matchDataAccess.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Test]
    public async Task AddAsync_ShouldAddMatchToDatabase()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var match = new Match
        {
            Id = matchId
        };

        // Act
        await _matchDataAccess.AddAsync(match);
        await _matchDataAccess.SaveChangesAsync();

        var result = await _dbContext.Matches.FindAsync(matchId);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Id, Is.EqualTo(matchId));
    }

    [Test]
    public void AddAsync_ShouldThrowArgumentNullException_WhenMatchIsNull()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _matchDataAccess.AddAsync(null);
        });
    }

    [Test]
    public async Task UpdateAsync_ShouldModifyMatchInDatabase()
    {
        // Arrange
        var existingMatch = new Match
        {
            Id = Guid.NewGuid(),
        };
        _dbContext.Matches.Add(existingMatch);
        await _dbContext.SaveChangesAsync();

        // Update match details
        existingMatch.Team1Score = 10;
        existingMatch.Team2Score = 5;

        // Act
        await _matchDataAccess.UpdateAsync(existingMatch);
        await _matchDataAccess.SaveChangesAsync();

        var result = await _dbContext.Matches.FindAsync(existingMatch.Id);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Team1Score, Is.EqualTo(10));
        Assert.That(result.Team2Score, Is.EqualTo(5));
    }

    [Test]
    public void UpdateAsync_ShouldThrowArgumentNullException_WhenMatchIsNull()
    {
        // Act & Assert
        Assert.ThrowsAsync<NullReferenceException>(async () =>
        {
            await _matchDataAccess.UpdateAsync(null);
        });
    }
}