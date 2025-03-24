using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KickStats.Repositories.Implementations;

public class PlayTableDataAccess : IPlayTableDataAccess
{
    private readonly ApplicationDbContext _dbContext;

    public PlayTableDataAccess(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PlayTable> GetByIdAsync(Guid id)
    {
        return await _dbContext.PlayTables
            .Include(pt => pt.Matches)
            .FirstOrDefaultAsync(pt => pt.Id == id);
    }

    public async Task AddAsync(PlayTable playTable)
    {
        await _dbContext.PlayTables.AddAsync(playTable);
    }

    public async Task UpdateAsync(PlayTable playTable)
    {
        _dbContext.PlayTables.Update(playTable);
    }

    public async Task<IEnumerable<Match>> GetMatchesForPlayTableAsync(Guid playTableId)
    {
        return await _dbContext.Matches
            .Where(m => m.PlayTableId == playTableId)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}