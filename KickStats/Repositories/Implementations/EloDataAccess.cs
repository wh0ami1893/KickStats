using KickStats.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using KickStats.Data;
using KickStats.Data.Models;

namespace KickStats.Repositories;

public class EloDataAccess : IEloDataAccess
{
    private readonly ApplicationDbContext _dbContext;

    public EloDataAccess(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Elo> GetEloByIdAsync(Guid id)
    {
        return await _dbContext.Elos.FindAsync(id) ?? throw new KeyNotFoundException($"Elo with ID {id} not found.");
    }

    public async Task<Elo> GetEloByPlayerIdAsync(Guid playerId)
    {
        var e = await _dbContext.Elos.FirstOrDefaultAsync(e => e.Player.Id == playerId);
        if(e == null)
        {
            throw new KeyNotFoundException($"Elo for Player {playerId} not found.");
        }

        return e;
    }


    public async Task AddEloAsync(Elo elo)
    {
        if (elo == null)
        {
            throw new ArgumentNullException(nameof(elo));
        }

        await _dbContext.Elos.AddAsync(elo);
    }

    public async Task UpdateEloAsync(Elo elo)
    {
        if (elo == null)
        {
            throw new ArgumentNullException(nameof(elo));
        }

        _dbContext.Elos.Update(elo);
    }

    public async Task DeleteEloAsync(Guid id)
    {
        var elo = await _dbContext.Elos.FindAsync(id);
        if (elo == null)
        {
            throw new KeyNotFoundException($"Elo with ID {id} not found.");
        }

        _dbContext.Elos.Remove(elo);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}