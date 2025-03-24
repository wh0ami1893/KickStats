using KickStats.Data.Models;

namespace KickStats.Repositories.Interfaces;

public interface IMatchDataAccess
{
    Task<Match> GetByIdAsync(Guid id);
    Task AddAsync(Match match);
    Task UpdateAsync(Match match);
    Task SaveChangesAsync();
}