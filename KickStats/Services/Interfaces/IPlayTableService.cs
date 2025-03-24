using KickStats.Data.Models;

namespace KickStats.Services.Interfaces;

public interface IPlayTableService
{
    Task<PlayTable> CreatePlayTableAsync(string name);
    Task<PlayTable> GetPlayTableByIdAsync(Guid id);
    Task OpenNewMatch(Guid playTableId, Match match);
    Task CloseMatchAsync(Guid playTableId, Match match);
    Task<IEnumerable<Match>> GetMatchesForPlayTableAsync(Guid playTableId);
}