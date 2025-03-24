using KickStats.Data;
using KickStats.Data.Models;

namespace KickStats.Repositories.Interfaces;

public interface ITeamDataAccess
{
    Task<Team> GetTeamByIdAsync(Guid id);
    Task<IEnumerable<Team>> GetAllTeamsAsync();
    Task<IEnumerable<Team>> GetTeamsByPlayerIdAsync(ApplicationUser player);
    Task AddTeamAsync(Team team);
    Task UpdateTeamAsync(Team team);
    Task DeleteTeamAsync(Guid id);
    Task SaveChangesAsync();
}