using KickStats.Data.Models;
using KickStats.Repositories.Interfaces;
using KickStats.Services.Interfaces;

namespace KickStats.Services.Implementations;

public class PlayTableService(IPlayTableDataAccess playTableDataAccess) : IPlayTableService
{
    private readonly IPlayTableDataAccess _playTableDataAccess = playTableDataAccess ?? throw new ArgumentNullException(nameof(playTableDataAccess));

    public async Task<PlayTable> CreatePlayTableAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("PlayTable name cannot be empty", nameof(name));

        var playTable = new PlayTable {Name = name };

        await _playTableDataAccess.AddAsync(playTable);
        await _playTableDataAccess.SaveChangesAsync();

        return playTable;
    }

    public async Task<PlayTable> GetPlayTableByIdAsync(Guid id)
    {
        var playTable = await _playTableDataAccess.GetByIdAsync(id);

        if (playTable == null)
            throw new KeyNotFoundException($"PlayTable with ID {id} not found.");

        return playTable;
    }

    public async Task OpenNewMatch(Guid playTableId, Match match)
    {
        var playTable = await GetPlayTableByIdAsync(playTableId);
        playTable.CurrentMatch = match;
        await _playTableDataAccess.UpdateAsync(playTable);
        await _playTableDataAccess.SaveChangesAsync();

    }

    public async Task CloseMatchAsync(Guid playTableId, Match match)
    {
        var playTable = await GetPlayTableByIdAsync(playTableId);
        if (playTable.CurrentMatch == match)
        {
            playTable.CurrentMatch = null;
            playTable.Matches.Add(match);
            await _playTableDataAccess.UpdateAsync(playTable);
            await _playTableDataAccess.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentException("Match is not the current match.");
        }
    }

    public async Task<IEnumerable<Match>> GetMatchesForPlayTableAsync(Guid playTableId)
    {
        return await _playTableDataAccess.GetMatchesForPlayTableAsync(playTableId);
    }



}