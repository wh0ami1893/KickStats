using KickStats.Data.Models;

namespace KickStats.Repositories.Interfaces;

public interface IEloDataAccess
{
    Task<Elo> GetEloByIdAsync(Guid id);
    Task<Elo> GetEloByPlayerIdAsync(Guid playerId);
    Task AddEloAsync(Elo elo);
    Task UpdateEloAsync(Elo elo);
    Task DeleteEloAsync(Guid id);
    Task SaveChangesAsync();
}