using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KickStats.Repositories.Implementations;

public class EloHistoryDataAccess(ApplicationDbContext _dbContext) : IEloHistoryDataAccess
{
    public async Task<EloHistory> GetEloHistoryByIdAsync(Guid id)
    {
        var eloHistory = await _dbContext.EloHistories.FirstOrDefaultAsync(eh => eh.Id == id) ?? throw new KeyNotFoundException($"EloHistory with ID {id} not found.");
        return eloHistory;
    }

    public async Task<IEnumerable<EloHistory>> GetEloHistoryByPlayerIdAsync(Guid playerId, int limit, int offset)
    {
        var eloHistories = await _dbContext.EloHistories.Where(eh => eh.PlayerId == playerId).Skip(offset).Take(limit).ToListAsync();
        return eloHistories.ToList();
    }

    public async Task<IEnumerable<EloHistory>> GetEloHistoryByPlayerIdAsync(Guid playerId, int limit)
    {
        var eloHistories = await _dbContext.EloHistories.Where(eh => eh.PlayerId == playerId).Take(limit).ToListAsync();
        return eloHistories.ToList();
    }

    public async Task<IEnumerable<EloHistory>> GetEloHistoryByPlayerIdAsync(Guid playerId)
    {
        var eloHistories = await _dbContext.EloHistories.Where(eh => eh.PlayerId == playerId).ToListAsync();
        return eloHistories.ToList();
    }

    public async Task AddEloHistoryAsync(EloHistory eloHistory)
    {
        await _dbContext.EloHistories.AddAsync(eloHistory);
    }

    public async Task DeleteEloHistoryAsync(Guid id)
    {
        var elo = await _dbContext.EloHistories.FindAsync(id);
        if (elo == null)
        {
            throw new KeyNotFoundException($"Elo with ID {id} not found.");
        }

        _dbContext.EloHistories.Remove(elo);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}