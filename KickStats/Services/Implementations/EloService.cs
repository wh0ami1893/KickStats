using KickStats.Data.Models;
using KickStats.Repositories.Interfaces;
using KickStats.Services.Interfaces;

namespace KickStats.Services.Implementations;

public class EloService(IEloDataAccess eloDataAccess, IEloHistoryDataAccess eloHistoryDataAccess) : IEloService
{
    public async Task<Elo> GetPlayerEloAsync(Guid playerId)
    {
        var elo = await eloDataAccess.GetEloByPlayerIdAsync(playerId);
        return elo;
    }

    public async Task UpdatePlayerEloAsync(Guid playerId, Match match)
    {
        var playerElo = await eloDataAccess.GetEloByPlayerIdAsync(playerId);

        var matchStats = match.PlayerStats.FirstOrDefault(x => x.PlayerId == playerId);
        if (matchStats == null)
        {
            throw new NullReferenceException($"Stats for Player {playerId} do not exist");
        }


        int ownScore = match.Team1.Players.Contains(playerElo.Player) ? match.Team1Score : match.Team2Score;
        int enemyScore = match.Team1.Players.Contains(playerElo.Player) ? match.Team2Score : match.Team1Score;

        var eloB = match.Team1.Players.Contains(playerElo.Player) ? match.Team2.Players.Average(p => p.CurrentElo.Score) : match.Team1.Players.Average(p => p.CurrentElo.Score);



        var ea = 1 / (1 + Math.Pow(10, (eloB - playerElo.Score) / 500));
        double sa;
        if (ownScore > enemyScore)
        {
            sa = 10.0 / (10.0 + enemyScore);
        }
        else
        {
            sa = 1 - 10.0 / (10.0 + ownScore);
        }

        var newElo = playerElo.Score + 100 * (sa - ea);

       await eloHistoryDataAccess.AddEloHistoryAsync(new EloHistory()
        {
            PlayerId = playerId,
            Score = playerElo.Score,
            Timestamp = DateTime.Now
        });
        await eloHistoryDataAccess.SaveChangesAsync();

        var updatedElo = new Elo()
        {
            Player = playerElo.Player,
            Score = (int) newElo
        };

        await eloDataAccess.AddEloAsync(updatedElo);
        await eloDataAccess.SaveChangesAsync();

        //src = "https://math.stackexchange.com/questions/838809/rating-system-for-2-vs-2-2-vs-1-and-1-vs-1-game"

    }

    public async Task<IEnumerable<EloHistory>> GetPlayerEloHistoryAsync(Guid playerId, int limit, int offset)
    {
        var eloHistory = await eloHistoryDataAccess.GetEloHistoryByPlayerIdAsync(playerId, limit, offset);
        return eloHistory;

    }

    public async Task<IEnumerable<EloHistory>> GetPlayerEloHistoryAsync(Guid playerId, int limit)
    {
        var eloHistory = await eloHistoryDataAccess.GetEloHistoryByPlayerIdAsync(playerId, limit);
        return eloHistory;
    }

    public async Task<IEnumerable<EloHistory>> GetPlayerEloHistoryAsync(Guid playerId)
    {
        var eloHistory = await eloHistoryDataAccess.GetEloHistoryByPlayerIdAsync(playerId);
        return eloHistory;
    }
}