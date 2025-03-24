using KickStats.Data.Models;

namespace KickStats.Services.Interfaces;

public interface IMatchService
{
    Task<Match> CreateMatchAsync(Guid playTableId, Guid team1Id, Guid team2Id, DateTime matchDate);
    Task<Match> GetMatchByIdAsync(Guid matchId);
    Task CloseMatchAsync(Guid matchId, int team1Score, int team2Score);
    Task<IEnumerable<Match>> GetMatchesForDateRangeAsync(DateTime startDate, DateTime endDate);
    Task UpdateMatchResultAsync(Guid matchId, int team1Score, int team2Score);
    Task DeleteMatchAsync(Guid matchId);
}