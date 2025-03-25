using KickStats.Data.Models;

namespace KickStats.Services.Interfaces;

public interface IEloService
{
    Task<Elo> GetPlayerEloAsync(Guid playerId);
    Task UpdatePlayerEloAsync(Guid playerId, Match match);
    Task<IEnumerable<EloHistory>> GetPlayerEloHistoryAsync(Guid playerId, int limit, int offset);
    Task<IEnumerable<EloHistory>> GetPlayerEloHistoryAsync(Guid playerId, int limit);
    Task<IEnumerable<EloHistory>> GetPlayerEloHistoryAsync(Guid playerId);
}