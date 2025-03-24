using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories.Interfaces;
using KickStats.Services.Implementations;
using Moq;
using static NUnit.Framework.Assert;
using Match = KickStats.Data.Models.Match;

namespace KickStatsTests.Services;

[TestFixture]
public class MatchServiceTests
{
    private Mock<IMatchDataAccess> _dataAccessMock;
    private MatchService _matchService;

    [SetUp]
    public void Setup()
    {
        _dataAccessMock = new Mock<IMatchDataAccess>();
        _matchService = new MatchService(_dataAccessMock.Object);
    }

    [Test]
    public async Task CreateMatchAsync_ShouldCreateMatch()
    {
        // Arrange
        var playTableId = Guid.NewGuid();
        var matchDate = DateTime.UtcNow;
        var createdMatch = new Match
        {
            PlayTableId = playTableId,
            State = MatchState.Open,
            Team1 = new Team(),
            Team2 = new Team()
        };

        _dataAccessMock.Setup(d => d.AddAsync(It.IsAny<Match>())).Returns(Task.CompletedTask);
        _dataAccessMock.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _matchService.CreateMatchAsync(playTableId, matchDate);

        // Assert

        Multiple(() =>
        {
            That(result, Is.Not.Null);
            That(result.PlayTableId, Is.EqualTo(playTableId));
            That(result.State, Is.EqualTo(MatchState.Open));
            That(result.Team1, Is.Not.Null);
            That(result.Team2, Is.Not.Null);
        });

        _dataAccessMock.Verify(d => d.AddAsync(It.IsAny<Match>()), Times.Once);
        _dataAccessMock.Verify(d => d.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task GetMatchByIdAsync_ShouldReturnMatch_WhenMatchExists()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var existingMatch = new Match { Id = matchId };

        _dataAccessMock.Setup(d => d.GetByIdAsync(matchId)).ReturnsAsync(existingMatch);

        // Act
        var result = await _matchService.GetMatchByIdAsync(matchId);
        That(result, Is.Not.Null);
        That(result.Id, Is.EqualTo(matchId));

        _dataAccessMock.Verify(d => d.GetByIdAsync(matchId), Times.Once);
    }

    [Test]
    public async Task GetMatchByIdAsync_ShouldReturnNull_WhenMatchDoesNotExist()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        _dataAccessMock.Setup(d => d.GetByIdAsync(matchId)).ReturnsAsync((Match)null);

        // Act
        var result = await _matchService.GetMatchByIdAsync(matchId);
        That(result, Is.Null);
        _dataAccessMock.Verify(d => d.GetByIdAsync(matchId), Times.Once);
    }

    [Test]
    public async Task StartMatchAsync_ShouldThrowArgumentException_WhenTeamsNotFull()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var match = new Match { Id = matchId, State = MatchState.Open };

        _dataAccessMock.Setup(d => d.GetByIdAsync(matchId)).ReturnsAsync(match);
        _dataAccessMock.Setup(d => d.UpdateAsync(It.IsAny<Match>())).Returns(Task.CompletedTask);
        _dataAccessMock.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        ThrowsAsync<ArgumentException>(() => _matchService.StartMatchAsync(matchId));
    }

    [Test]
    public async Task StartMatchRandomizedAsync_ShouldThrowArgumentException_WhenTeamsNotFull()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var match = new Match { Id = matchId, State = MatchState.Open };

        _dataAccessMock.Setup(d => d.GetByIdAsync(matchId)).ReturnsAsync(match);
        _dataAccessMock.Setup(d => d.UpdateAsync(It.IsAny<Match>())).Returns(Task.CompletedTask);
        _dataAccessMock.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        ThrowsAsync<ArgumentException>(() => _matchService.StartMatchRandomizedAsync(matchId));
    }

    [Test]
    public async Task StartMatchRandomizedAsync_ShouldDistributePlayersAcrossTeamsRandomly()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var players = new List<ApplicationUser>
        {
            new ApplicationUser { Id = Guid.NewGuid(), UserName = "Player1" },
            new ApplicationUser { Id = Guid.NewGuid(), UserName = "Player2" },
            new ApplicationUser { Id = Guid.NewGuid(), UserName = "Player3" },
            new ApplicationUser { Id = Guid.NewGuid(), UserName = "Player4" }
        };

        var match = new Match
        {
            Id = matchId,
            Team1 = new Team { Players = new List<ApplicationUser>() },
            Team2 = new Team { Players = new List<ApplicationUser>() }
        };

        match.Team1.Players.Add(players[0]); // Start with one player in Team1
        match.Team1.Players.Add(players[1]); // Start with one player in Team1
        match.Team2.Players.Add(players[2]); // Start with one player in Team2
        match.Team2.Players.Add(players[3]); // Start with one player in Team2

        _dataAccessMock.Setup(d => d.GetByIdAsync(matchId)).ReturnsAsync(match);
        _dataAccessMock.Setup(d => d.UpdateAsync(It.IsAny<Match>())).Returns(Task.CompletedTask);
        _dataAccessMock.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _matchService.StartMatchRandomizedAsync(matchId);

        Assert.Multiple(() =>
        {
            That(match.Team1.Players.Count, Is.EqualTo(2)); // Randomized two players in each team
            That(match.Team2.Players.Count, Is.EqualTo(2));
            That(match.Team2.Players, Is.Not.EqualTo(match.Team1.Players)); // Ensure randomized distribution
            That(match.State, Is.EqualTo(MatchState.Running));
            That(match.StartTime, Is.Not.EqualTo(default(DateTime)));
        });

        _dataAccessMock.Verify(d => d.UpdateAsync(It.IsAny<Match>()), Times.Once);
        _dataAccessMock.Verify(d => d.SaveChangesAsync(), Times.Once);
    }


    [Test]
    public async Task CloseMatchAsync_ShouldSetMatchToClosedStateAndSaveScores()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var match = new Match { Id = matchId, State = MatchState.Running };

        _dataAccessMock.Setup(d => d.GetByIdAsync(matchId)).ReturnsAsync(match);
        _dataAccessMock.Setup(d => d.UpdateAsync(It.IsAny<Match>())).Returns(Task.CompletedTask);
        _dataAccessMock.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask);

        var team1Score = 10;
        var team2Score = 5;

        // Act
        await _matchService.CloseMatchAsync(matchId, team1Score, team2Score);

        // Assert
        NotNull(match.EndTime);
        AreEqual(team1Score, match.Team1Score);
        AreEqual(team2Score, match.Team2Score);

        _dataAccessMock.Verify(d => d.UpdateAsync(It.IsAny<Match>()), Times.Once);
        _dataAccessMock.Verify(d => d.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void CloseMatchAsync_ShouldThrowException_WhenMatchDoesNotExist()
    {
        // Arrange
        var matchId = Guid.NewGuid();

        _dataAccessMock.Setup(d => d.GetByIdAsync(matchId)).ReturnsAsync((Match)null);

        // Act & Assert
        var ex = ThrowsAsync<KeyNotFoundException>(async () =>
            await _matchService.CloseMatchAsync(matchId, 10, 5));
        AreEqual($"Match with ID {matchId} not found.", ex.Message);
    }

    [Test]
    public async Task JoinMatchAsync_ShouldAddPlayerToSpecifiedTeam()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var player = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Player1" };
        var match = new Match { Id = matchId, State = MatchState.Open, Team1 = new Team(), Team2 = new Team() };

        _dataAccessMock.Setup(d => d.GetByIdAsync(matchId)).ReturnsAsync(match);

        // Act
        await _matchService.JoinMatchAsync(matchId, player, 1);
        That(match.Team1.Players.Contains(player), Is.True);
        _dataAccessMock.Verify(d => d.UpdateAsync(It.IsAny<Match>()), Times.Once);
        _dataAccessMock.Verify(d => d.SaveChangesAsync(), Times.Never); // Changes saved later in the workflow
    }

    [Test]
    public void JoinMatchAsync_ShouldThrowException_WhenTeamIsFull()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var player = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Player1" };
        var match = new Match
        {
            Id = matchId,
            Team1 = new Team { Players = new List<ApplicationUser> { new(), new() } }, // Full team
            Team2 = new Team()
        };

        _dataAccessMock.Setup(d => d.GetByIdAsync(matchId)).ReturnsAsync(match);

        // Act & Assert
        var ex = ThrowsAsync<ArgumentException>(async () =>
            await _matchService.JoinMatchAsync(matchId, player, 1));
        AreEqual("Team is full.", ex.Message);
    }

    [Test]
    public async Task LeaveMatchAsync_ShouldRemovePlayerFromSpecifiedTeam()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var player = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Player1" };
        var match = new Match { Id = matchId, Team1 = new Team { Players = new List<ApplicationUser> { player } }, Team2 = new Team() };

        _dataAccessMock.Setup(d => d.GetByIdAsync(matchId)).ReturnsAsync(match);
        _dataAccessMock.Setup(d => d.UpdateAsync(It.IsAny<Match>())).Returns(Task.CompletedTask);

        // Act
        await _matchService.LeaveMatchAsync(matchId, player, 1);

        // Assert
        IsFalse(match.Team1.Players.Contains(player));
        _dataAccessMock.Verify(d => d.UpdateAsync(It.IsAny<Match>()), Times.Once);
    }

    [Test]
    public void LeaveMatchAsync_ShouldThrowException_WhenInvalidTeamNumber()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var player = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Player1" };
        var match = new Match { Id = matchId, Team1 = new Team(), Team2 = new Team() };

        _dataAccessMock.Setup(d => d.GetByIdAsync(matchId)).ReturnsAsync(match);

        // Act & Assert
        var ex = ThrowsAsync<ArgumentException>(async () =>
            await _matchService.LeaveMatchAsync(matchId, player, 3));
        AreEqual("Invalid team number.", ex.Message);
    }
}