using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KickStats.Repositories.Implementations;

public class TeamDataAccess : ITeamDataAccess
{
    private readonly ApplicationDbContext _dbContext;

    public TeamDataAccess(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Retrieve a specific team by its unique ID.
    /// </summary>
    public async Task<Team> GetTeamByIdAsync(Guid id)
    {
        return await _dbContext.Teams.FindAsync(id);
    }

    /// <summary>
    /// Retrieve all teams from the database.
    /// </summary>
    public async Task<IEnumerable<Team>> GetAllTeamsAsync()
    {
        return await _dbContext.Teams.ToListAsync();
    }


    /// <summary>
    /// Retrieve teams associated with a specific player by their PlayerId.
    /// </summary>
    public async Task<IEnumerable<Team>> GetTeamsByPlayerIdAsync(ApplicationUser player)
    {
        return await _dbContext.Teams
            .Where(t => t.Players.Contains(player)) // Check if the PlayerId exists in the team
            .ToListAsync();
    }

    /// <summary>
    /// Add a new team to the database.
    /// </summary>
    public async Task AddTeamAsync(Team team)
    {
        if (team == null)
        {
            throw new ArgumentNullException(nameof(team));
        }

        await _dbContext.Teams.AddAsync(team);
    }

    /// <summary>
    /// Update team details.
    /// </summary>
    public async Task UpdateTeamAsync(Team team)
    {
        if (team == null)
        {
            throw new ArgumentNullException(nameof(team));
        }

        _dbContext.Teams.Update(team);
    }

    /// <summary>
    /// Delete a team by its unique ID.
    /// </summary>
    public async Task DeleteTeamAsync(Guid id)
    {
        var team = await _dbContext.Teams.FindAsync(id);
        if (team == null)
        {
            throw new KeyNotFoundException($"Team with ID {id} was not found.");
        }

        _dbContext.Teams.Remove(team);
    }

    /// <summary>
    /// Save any pending changes to the database.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}