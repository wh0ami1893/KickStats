using KickStats.Data;
using KickStats.Data.Models;

namespace KickStats.Services.Interfaces;

public interface IMatchService
{
    Task<Match> CreateMatchAsync(Guid playTableId, DateTime matchDate);
    Task<Match> GetMatchByIdAsync(Guid matchId);
    Task StartMatchAsync(Guid matchId);
    Task StartMatchRandomizedAsync(Guid matchId);
    Task CloseMatchAsync(Guid matchId, int team1Score, int team2Score, List<PlayerMatchStats> matchStats);
    Task JoinMatchAsync(Guid matchId, ApplicationUser player, int teamNumber);
}