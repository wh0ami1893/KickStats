using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace KickStatsTests.Repositories;

[TestFixture]
public class TeamDataAccessTests
{
    private ApplicationDbContext _dbContext;
    private TeamDataAccess _teamDataAccess;

    [SetUp]
    public void SetUp()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabaseTeams")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _teamDataAccess = new TeamDataAccess(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        // Dispose database to ensure isolation between tests
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }


    [Test]
    public async Task AddTeamAsync_ShouldAddTeam()
    {
        // Arrange

        var player1 = new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            UserName = "TestPlayer1",
            Email = "<EMAIL>"
        };
        var player2 = new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            UserName = "TestPlayer2",
            Email = "<EMAIL>"
        };

        var team = new Team
        {
            Id = Guid.NewGuid(),
            Players = [player1, player2],
        };

        // Act
        await _teamDataAccess.AddTeamAsync(team);
        await _teamDataAccess.SaveChangesAsync();

        // Assert
        var result = await _dbContext.Teams.FirstOrDefaultAsync(t => t.Id == team.Id);
        Assert.NotNull(result);
        Assert.That(result.Id, Is.EqualTo(team.Id));
    }

    [Test]
    public void AddTeamAsync_ShouldThrowArgumentNullException_ForNullTeam()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _teamDataAccess.AddTeamAsync(null));
    }

    [Test]
    public async Task GetTeamByIdAsync_ShouldReturnTeam_WhenTeamExists()
    {
        // Arrange
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Players = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    Id = Guid.NewGuid(),
                    UserName = "TestPlayer1",
                    Email = "<EMAIL>"
                },
                new ApplicationUser()
                {
                    Id = Guid.NewGuid(),
                    UserName = "TestPlayer2",
                    Email = "<EMAIL>"
                }
            }
        };

        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _teamDataAccess.GetTeamByIdAsync(team.Id);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.Id, Is.EqualTo(team.Id));
    }

    [Test]
    public async Task GetTeamByIdAsync_ShouldReturnNull_WhenTeamDoesNotExist()
    {
        // Act
        var result = await _teamDataAccess.GetTeamByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Test]
    public async Task GetAllTeamsAsync_ShouldReturnAllTeams()
    {
        // Arrange
        var team1 = new Team { Id = Guid.NewGuid() };
        var team2 = new Team { Id = Guid.NewGuid()};

        _dbContext.Teams.AddRange(team1, team2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _teamDataAccess.GetAllTeamsAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task UpdateTeamAsync_ShouldUpdateTeam()
    {
        // Arrange
        var player1 = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "TestPlayer1",
            Email = "player1@test.com"
        };
        var player2 = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "TestPlayer2",
            Email = "player2@test.com"
        };
        var team = new Team
        {
            Players = new List<ApplicationUser>
            {
                player1, player2
            }
        };

        // Add the team to the database
        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync();

        // Fetch the team from the database to ensure it's tracked by the DbContext
        var addedTeam = await _dbContext.Teams
            .Include(t => t.Players) // Ensure the Players collection is loaded
            .FirstOrDefaultAsync();

        var player3 = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "TestPlayer3",
            Email = "player3@test.com"
        };

        // Update the team
        _dbContext.Users.Add(player3); // Add a new player
        await _dbContext.SaveChangesAsync();

        addedTeam.Players.Remove(player2); // Remove an existing player
        addedTeam.Players.Add(player3);   // Add a new player

        // Act
        await _teamDataAccess.UpdateTeamAsync(addedTeam);
        await _teamDataAccess.SaveChangesAsync();

        // Assert
        var updatedTeam = await _dbContext.Teams
            .Include(t => t.Players) // Ensure the Players collection is loaded
            .FirstOrDefaultAsync(t => t.Id == team.Id);

        Assert.That(updatedTeam, Is.Not.Null);
        Assert.That(updatedTeam.Id, Is.EqualTo(addedTeam.Id));
        Assert.That(updatedTeam.Players.Count, Is.EqualTo(2));
        Assert.That(updatedTeam.Players.Contains(player1), Is.True);
        Assert.That(updatedTeam.Players.Contains(player3), Is.True);
    }

    [Test]
    public void UpdateTeamAsync_ShouldThrowArgumentNullException_ForNullTeam()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _teamDataAccess.UpdateTeamAsync(null));
    }

    [Test]
    public async Task DeleteTeamAsync_ShouldDeleteTeam()
    {
        // Arrange
        var player1 = new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            UserName = "TestPlayer1",
            Email = "<EMAIL>"
        };
        var player2 = new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            UserName = "TestPlayer2",
            Email = "<EMAIL>"
        };
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Players = new List<ApplicationUser>()
            {
                player1, player2
            }
        };

        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync();

        // Act
        await _teamDataAccess.DeleteTeamAsync(team.Id);
        await _teamDataAccess.SaveChangesAsync();

        var deletedTeam = await _dbContext.Teams.FirstOrDefaultAsync(t => t.Id == team.Id);

        // Assert
        Assert.Null(deletedTeam);
    }

    [Test]
    public void DeleteTeamAsync_ShouldThrowKeyNotFoundException_WhenTeamDoesNotExist()
    {
        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            await _teamDataAccess.DeleteTeamAsync(Guid.NewGuid());
        });
    }

    [Test]
    public async Task GetTeamsByPlayerIdAsync_ShouldReturnTeamsForPlayer()
    {
        // Arrange
        var player = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Player1" };
        var player2 = new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            UserName = "TestPlayer2",
            Email = "<EMAIL>"
        };
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Players = new List<ApplicationUser>()
            {
                player, player2
            }
        };
        var team2 = new Team
        {
            Id = Guid.NewGuid(),
            Players = new List<ApplicationUser> { player }
        };

        _dbContext.Teams.AddRange(team, team2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _teamDataAccess.GetTeamsByPlayerIdAsync(player);

        // Assert
        Assert.AreEqual(2, result.Count());
        Assert.That(result.Contains(team));
        Assert.That(result.Contains(team2));
    }
}