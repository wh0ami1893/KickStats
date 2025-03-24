using KickStats.Data.Models;

namespace KickStats.Repositories.Interfaces;

public interface IPlayTableDataAccess
{
    Task<PlayTable> GetByIdAsync(Guid id);
    Task AddAsync(PlayTable playTable);
    Task UpdateAsync(PlayTable playTable);
    Task<IEnumerable<Match>> GetMatchesForPlayTableAsync(Guid playTableId);
    Task SaveChangesAsync();
}