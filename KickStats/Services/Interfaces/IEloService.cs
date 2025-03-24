using KickStats.Data.Models;

namespace KickStats.Services.Interfaces;

public interface IEloService
{
    Task<Elo> GetPlayerEloAsync(Guid playerId);
    Task UpdatePlayerEloAsync(Guid playerId, int newScore);
    Task<IEnumerable<EloHistory>> GetPlayerEloHistoryAsync(Guid playerId);
}