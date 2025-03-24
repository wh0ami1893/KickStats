using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace KickStatsTests.Repositories
{
    [TestFixture]
    public class PlayerMatchStatsDataAccessTests
    {
        private ApplicationDbContext _context;
        private PlayerMatchStatsDataAccess _dataAccess;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for every test
                .Options;

            _context = new ApplicationDbContext(options);
            _dataAccess = new PlayerMatchStatsDataAccess(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        #region AddPlayerStatAsync Tests

        [Test]
        public async Task AddPlayerStatAsync_ShouldAddNewPlayerStat()
        {
            // Arrange
            var playerMatchStats = new PlayerMatchStats
            {
                Id = Guid.NewGuid(),
                MatchId = Guid.NewGuid(),
                PlayerId = Guid.NewGuid(),
                Points = 10
            };

            // Act
            await _dataAccess.AddPlayerStatAsync(playerMatchStats);
            await _dataAccess.SaveChangesAsync();

            // Assert
            var result = await _context.PlayerMatchStats.FindAsync(playerMatchStats.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(playerMatchStats.Points, result.Points);
        }

        [Test]
        public void AddPlayerStatAsync_ShouldThrowArgumentNullException_WhenPlayerStatIsNull()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _dataAccess.AddPlayerStatAsync(null);
            });
        }

        #endregion

        #region GetPlayerStatsByMatchIdAsync Tests

        [Test]
        public async Task GetPlayerStatsByMatchIdAsync_ShouldReturnMatchingStats()
        {
            // Arrange
            var matchId = Guid.NewGuid();
            Match m = new Match { Id = matchId };
            _context.Matches.Add(m);
            await _context.SaveChangesAsync();

            var player = new ApplicationUser { Id = Guid.NewGuid()};
            var player2 = new ApplicationUser { Id = Guid.NewGuid()};
            _context.Users.AddRange(player, player2);
            await _context.SaveChangesAsync();

            var playerStats = new List<PlayerMatchStats>
            {
                new PlayerMatchStats { Id = Guid.NewGuid(), Match = m, Points = 15, Player = player },
                new PlayerMatchStats { Id = Guid.NewGuid(), Match = m, Points = 20, Player = player2}
            };



            await _context.PlayerMatchStats.AddRangeAsync(playerStats);
            await _context.SaveChangesAsync();

            // Act
            var result = await _dataAccess.GetPlayerStatsByMatchIdAsync(matchId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.IsTrue(result.All(pms => pms.MatchId == matchId));
        }

        [Test]
        public async Task GetPlayerStatsByMatchIdAsync_ShouldReturnEmpty_WhenNoMatchesFound()
        {
            // Act
            var result = await _dataAccess.GetPlayerStatsByMatchIdAsync(Guid.NewGuid());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        #endregion

        #region GetPlayerStatsByIdAsync Tests

        [Test]
        public async Task GetPlayerStatsByIdAsync_ShouldReturnPlayerStat_WhenIdIsValid()
        {
            // Arrange
            var m = new Match { Id = Guid.NewGuid() };
            _context.Matches.Add(m);
            await _context.SaveChangesAsync();
            var p = new ApplicationUser { Id = Guid.NewGuid()};
            _context.Users.Add(p);
            await _context.SaveChangesAsync();

            var playerMatchStats = new PlayerMatchStats
            {
                Match = m,
                Player = p,
                Points = 10
            };

           var pms =  await _context.PlayerMatchStats.AddAsync(playerMatchStats);
            await _context.SaveChangesAsync();

            // Act
            var result = await _dataAccess.GetByIdAsync(pms.Entity.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Points, Is.EqualTo(playerMatchStats.Points));
        }

        [Test]
        public async Task GetPlayerStatsByIdAsync_ShouldReturnNull_WhenIdIsInvalid()
        {
            // Act
            var result = await _dataAccess.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region UpdatePlayerStatAsync Tests

        [Test]
        public async Task UpdatePlayerStatAsync_ShouldUpdateExistingPlayerStat()
        {
            // Arrange
            var playerMatchStats = new PlayerMatchStats
            {
                Id = Guid.NewGuid(),
                MatchId = Guid.NewGuid(),
                PlayerId = Guid.NewGuid(),
                Points = 10
            };

            await _context.PlayerMatchStats.AddAsync(playerMatchStats);
            await _context.SaveChangesAsync();

            // Act
            playerMatchStats.Points = 20;
            await _dataAccess.UpdatePlayerStatAsync(playerMatchStats);
            await _dataAccess.SaveChangesAsync();

            // Assert
            var result = await _context.PlayerMatchStats.FindAsync(playerMatchStats.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Points, Is.EqualTo(20));
        }

        [Test]
        public void UpdatePlayerStatAsync_ShouldThrowArgumentNullException_WhenPlayerStatIsNull()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _dataAccess.UpdatePlayerStatAsync(null);
            });
        }

        #endregion

        #region DeletePlayerStatAsync Tests

        [Test]
        public async Task DeletePlayerStatAsync_ShouldRemovePlayerStat_WhenIdIsValid()
        {
            // Arrange
            var playerMatchStats = new PlayerMatchStats
            {
                Id = Guid.NewGuid(),
                MatchId = Guid.NewGuid(),
                PlayerId = Guid.NewGuid(),
                Points = 10
            };

            await _context.PlayerMatchStats.AddAsync(playerMatchStats);
            await _context.SaveChangesAsync();

            // Act
            await _dataAccess.DeletePlayerStatAsync(playerMatchStats.Id);
            await _dataAccess.SaveChangesAsync();

            // Assert
            var result = await _context.PlayerMatchStats.FindAsync(playerMatchStats.Id);
            Assert.IsNull(result);
        }

        [Test]
        public async Task DeletePlayerStatAsync_ShouldDoNothing_WhenIdIsInvalid()
        {
            // Act
            await _dataAccess.DeletePlayerStatAsync(Guid.NewGuid());

            // Assert (no exception thrown, no other side effects occur)
            Assert.Pass();
        }

        #endregion

   }
}