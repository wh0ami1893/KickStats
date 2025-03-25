using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories.Interfaces;
using KickStats.Services.Implementations;
using Moq;
using Match = KickStats.Data.Models.Match;

namespace KickStatsTests.Services
{
    [TestFixture]
    public class MatchServiceTests
    {
        private MatchService _matchService;

        // Mocked Data Access Dependencies
        private Mock<IMatchDataAccess> _mockMatchDataAccess;
        private Mock<IPlayerMatchStatsDataAccess> _mockPlayerMatchStatsDataAccess;
        private Mock<IPlayTableDataAccess> _mockPlayTableDataAccess;

        [SetUp]
        public void SetUp()
        {
            // Initialize mocked dependencies
            _mockMatchDataAccess = new Mock<IMatchDataAccess>();
            _mockPlayerMatchStatsDataAccess = new Mock<IPlayerMatchStatsDataAccess>();
            _mockPlayTableDataAccess = new Mock<IPlayTableDataAccess>();

            // Initialize service with mocks
            _matchService = new MatchService(
                _mockMatchDataAccess.Object, _mockPlayTableDataAccess.Object
            );
        }

        #region CreateMatchAsync Tests

        [Test]
        public async Task CreateMatchAsync_ShouldAddAndReturnNewMatch()
        {
            // Arrange
            var playTableId = Guid.NewGuid();
            var matchDate = DateTime.UtcNow;

            var playTable = new PlayTable { Id = playTableId };
            var match = new Match
            {
                Id = Guid.NewGuid(),
                PlayTableId = playTableId,
                StartTime = matchDate
            };

            _mockPlayTableDataAccess
                .Setup(x => x.GetByIdAsync(playTableId))
                .ReturnsAsync(playTable);

            _mockMatchDataAccess
                .Setup(x => x.AddAsync(It.IsAny<Match>()))
                .Callback<Match>(m => m.Id = match.Id); // Simulate setting the ID
            _mockMatchDataAccess
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _matchService.CreateMatchAsync(playTableId, matchDate);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(match.Id));
            _mockMatchDataAccess.Verify(x => x.AddAsync(It.IsAny<Match>()), Times.Once);
            _mockMatchDataAccess.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void CreateMatchAsync_ShouldThrowException_WhenPlayTableDoesNotExist()
        {
            // Arrange
            var playTableId = Guid.NewGuid();

            _mockPlayTableDataAccess
                .Setup(x => x.GetByIdAsync(playTableId))
                .ReturnsAsync((PlayTable)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _matchService.CreateMatchAsync(playTableId, DateTime.UtcNow));
        }

        #endregion

        #region GetMatchByIdAsync Tests

        [Test]
        public async Task GetMatchByIdAsync_ShouldReturnMatch_WhenMatchExists()
        {
            // Arrange
            var matchId = Guid.NewGuid();
            var expectedMatch = new Match { Id = matchId };

            _mockMatchDataAccess
                .Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(expectedMatch);

            // Act
            var result = await _matchService.GetMatchByIdAsync(matchId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMatch, result);
            _mockMatchDataAccess.Verify(x => x.GetByIdAsync(matchId), Times.Once);
        }

        [Test]
        public void GetMatchByIdAsync_ShouldThrowException_WhenMatchDoesNotExist()
        {
            // Arrange
            var matchId = Guid.NewGuid();

            _mockMatchDataAccess
                .Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync((Match)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _matchService.GetMatchByIdAsync(matchId));
        }

        #endregion

        #region StartMatchAsync Tests

        [Test]
        public async Task StartMatchAsync_ShouldUpdateMatchState_ToRunning()
        {
            // Arrange
            var matchId = Guid.NewGuid();
            var team1 = new Team()
            {
                Players = [new ApplicationUser(), new ApplicationUser()]
            };
            var team2 = new Team()
            {
                Players = [new ApplicationUser(), new ApplicationUser()]
            };;
            var match = new Match { Id = matchId, State = MatchState.Open, Team1 = team1, Team2 = team2};

            _mockMatchDataAccess
                .Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);

            _mockMatchDataAccess
                .Setup(x => x.UpdateAsync(It.IsAny<Match>()))
                .Returns(Task.CompletedTask);
            _mockMatchDataAccess
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _matchService.StartMatchAsync(matchId);

            var updatedMatch = await _mockMatchDataAccess.Object.GetByIdAsync(matchId);

            // Assert
            Assert.That(updatedMatch.State, Is.EqualTo(MatchState.Running));
            _mockMatchDataAccess.Verify(x => x.UpdateAsync(It.Is<Match>(m => m.State == MatchState.Running)), Times.Once);
            _mockMatchDataAccess.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void StartMatchAsync_ShouldThrowException_WhenMatchDoesNotExist()
        {
            // Arrange
            var matchId = Guid.NewGuid();

            _mockMatchDataAccess
                .Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync((Match)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _matchService.StartMatchAsync(matchId));
        }

        #endregion

        #region CloseMatchAsync Tests

        [Test]
        public async Task CloseMatchAsync_ShouldUpdateMatchScores()
        {
            // Arrange
            var matchId = Guid.NewGuid();
            var match = new Match { Id = matchId, State = MatchState.Running };

            var playerStats = new List<PlayerMatchStats>
            {
                new PlayerMatchStats { Id = Guid.NewGuid(), Points = 10 },
                new PlayerMatchStats { Id = Guid.NewGuid(), Points = 15 }
            };

            _mockMatchDataAccess
                .Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync(match);
            _mockPlayerMatchStatsDataAccess
                .Setup(x => x.AddPlayerStatAsync(It.IsAny<PlayerMatchStats>()))
                .Returns(Task.CompletedTask);
            _mockPlayerMatchStatsDataAccess
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mockMatchDataAccess
                .Setup(x => x.UpdateAsync(It.IsAny<Match>()))
                .Returns(Task.CompletedTask);
            _mockMatchDataAccess
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _matchService.CloseMatchAsync(matchId, 20, 15, playerStats);

            // Assert
            Assert.That(match.State, Is.EqualTo(MatchState.Finished));
            Assert.That(match.Team1Score, Is.EqualTo(20));
            Assert.That(match.Team2Score, Is.EqualTo(15));
            _mockMatchDataAccess.Verify(x => x.UpdateAsync(It.Is<Match>(m =>
                m.State == MatchState.Finished && m.Team1Score == 20 && m.Team2Score == 15
            )), Times.Once);
            _mockMatchDataAccess.Verify(x => x.SaveChangesAsync(), Times.Once);
            _mockPlayerMatchStatsDataAccess.Verify(x => x.AddPlayerStatAsync(It.IsAny<PlayerMatchStats>()), Times.Exactly(playerStats.Count));
        }

        [Test]
        public void CloseMatchAsync_ShouldThrowException_WhenMatchDoesNotExist()
        {
            // Arrange
            var matchId = Guid.NewGuid();

            _mockMatchDataAccess
                .Setup(x => x.GetByIdAsync(matchId))
                .ReturnsAsync((Match)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _matchService.CloseMatchAsync(matchId, 20, 15, new List<PlayerMatchStats>()));
        }

        #endregion
    }
}