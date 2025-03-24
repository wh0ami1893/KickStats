using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KickStats.Repositories.Implementations;

public class MatchDataAccess(ApplicationDbContext dbContext) : IMatchDataAccess
{
    public ApplicationDbContext _dbContext { get; set; } = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<Match> GetByIdAsync(Guid id)
    {
        return await _dbContext.Matches
            .FirstOrDefaultAsync(pt => pt.Id == id);
    }

    public async Task AddAsync(Match match)
    {
        await _dbContext.Matches.AddAsync(match);
    }

    public async Task UpdateAsync(Match match)
    {
        _dbContext.Matches.Update(match);
    }


    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}