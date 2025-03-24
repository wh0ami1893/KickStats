using KickStats.Data.Models;

namespace KickStats.Repositories.Interfaces;

public interface IEloHistoryDataAccess
{
    Task<EloHistory> GetEloHistoryByIdAsync(Guid id);
    Task<IEnumerable<EloHistory>> GetEloHistoryByPlayerIdAsync(Guid playerId, int limit, int offset);
    Task<IEnumerable<EloHistory>> GetEloHistoryByPlayerIdAsync(Guid playerId, int limit);
    Task<IEnumerable<EloHistory>> GetEloHistoryByPlayerIdAsync(Guid playerId);
    Task AddEloHistoryAsync(EloHistory eloHistory);
    Task DeleteEloHistoryAsync(Guid id);
    Task SaveChangesAsync();
}