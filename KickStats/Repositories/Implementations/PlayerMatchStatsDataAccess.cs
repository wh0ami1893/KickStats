using KickStats.Data.Models;
using KickStats.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KickStats.Data;

namespace KickStats.Repositories.Implementations
{
    public class PlayerMatchStatsDataAccess : IPlayerMatchStatsDataAccess
    {
        private readonly ApplicationDbContext _context;

        public PlayerMatchStatsDataAccess(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<PlayerMatchStats>> GetPlayerStatsByMatchIdAsync(Guid matchId)
        {
            // Retrieve all stats for the given match, including related player data
            return await _context.PlayerMatchStats
                .Include(pms => pms.Player)
                .Where(pms => pms.MatchId == matchId)
                .ToListAsync();
        }

        public async Task<PlayerMatchStats?> GetByIdAsync(Guid id)
        {
            // Retrieve specific player match stats by ID, including related entities (Player/Match)
            var result =  await _context.PlayerMatchStats
                .Include(pms => pms.Player)
                .Include(pms => pms.Match)
                .FirstOrDefaultAsync(pms => pms.Id == id);

            return result;
        }

        public async Task AddPlayerStatAsync(PlayerMatchStats playerMatchStats)
        {
            // Ensure that the playerMatchStats is not null
            if (playerMatchStats == null)
            {
                throw new ArgumentNullException(nameof(playerMatchStats));
            }

            // Add the stats to the database
            await _context.PlayerMatchStats.AddAsync(playerMatchStats);
        }

        public async Task UpdatePlayerStatAsync(PlayerMatchStats playerMatchStats)
        {
            // Update the existing stats
            if (playerMatchStats == null)
            {
                throw new ArgumentNullException(nameof(playerMatchStats));
            }

            _context.PlayerMatchStats.Update(playerMatchStats);
        }

        public async Task DeletePlayerStatAsync(Guid id)
        {
            // Retrieve the stats first
            var playerMatchStats = await _context.PlayerMatchStats.FindAsync(id);

            // If found, remove it
            if (playerMatchStats != null)
            {
                _context.PlayerMatchStats.Remove(playerMatchStats);
            }
        }

        public async Task SaveChangesAsync()
        {
            // Save all pending changes to the database
            await _context.SaveChangesAsync();
        }
    }
}