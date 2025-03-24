using KickStats.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickStats.Repositories.Interfaces
{
    public interface IPlayerMatchStatsDataAccess
    {
        Task<IEnumerable<PlayerMatchStats>> GetPlayerStatsByMatchIdAsync(Guid matchId);
        Task<PlayerMatchStats?> GetByIdAsync(Guid id);
        Task AddPlayerStatAsync(PlayerMatchStats playerMatchStats);
        Task UpdatePlayerStatAsync(PlayerMatchStats playerMatchStats);
        Task DeletePlayerStatAsync(Guid id);
        Task SaveChangesAsync();
    }
}