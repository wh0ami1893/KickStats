using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories.Interfaces;
using KickStats.Services.Interfaces;

namespace KickStats.Services.Implementations;

public class MatchService(IMatchDataAccess matchDataAccess, IPlayTableDataAccess playTableDataAccess ) : IMatchService
{
    public async Task<Match> CreateMatchAsync(Guid playTableId, DateTime matchDate)
    {
        var playTable = await playTableDataAccess.GetByIdAsync(playTableId);
        if (playTable == null)
        {
            throw new KeyNotFoundException($"PlayTable with ID {playTableId} not found.");
        }
        var match = new Match()
        {
            PlayTable = playTable,
            Team1 = new Team(),
            Team2 = new Team(),
            State = MatchState.Open
        };
        await matchDataAccess.AddAsync(match);
        await matchDataAccess.SaveChangesAsync();
        return match;
    }

    public async Task<Match> GetMatchByIdAsync(Guid matchId)
    {
        var match = await matchDataAccess.GetByIdAsync(matchId);
        if (match == null)
        {
            throw new KeyNotFoundException($"Match with ID {matchId} not found.");
        }
        return match;
    }

    public async Task StartMatchAsync(Guid matchId)
    {
       var match = await GetMatchByIdAsync(matchId);

       if (match.State != MatchState.Open || match.Team1.Players.Count < 2 || match.Team2.Players.Count < 2)
       {
           throw new ArgumentException("Match is not open or not ready to start.");
       }

       match.StartTime = DateTime.Now;
       match.State = MatchState.Running;
       await matchDataAccess.UpdateAsync(match);
       await matchDataAccess.SaveChangesAsync();
    }

    public async Task StartMatchRandomizedAsync(Guid matchId)
    {
        var match = await GetMatchByIdAsync(matchId);
        match.StartTime = DateTime.Now;

        var joinedPlayers = new List<ApplicationUser>();
        joinedPlayers.AddRange(match.Team1.Players);
        joinedPlayers.AddRange(match.Team2.Players);

        Random rnd = new Random();
        match.Team1.Players.Add(joinedPlayers[rnd.Next(0, joinedPlayers.Count)]);
        match.Team1.Players.Add(joinedPlayers[rnd.Next(0, joinedPlayers.Count)]);
        match.Team2.Players.Add(joinedPlayers[rnd.Next(0, joinedPlayers.Count)]);
        match.Team2.Players.Add(joinedPlayers[rnd.Next(0, joinedPlayers.Count)]);

        await matchDataAccess.UpdateAsync(match);
        await matchDataAccess.SaveChangesAsync();
    }

    public async Task CloseMatchAsync(Guid matchId, int team1Score, int team2Score, List<PlayerMatchStats> matchStats)
    {
        var match = await matchDataAccess.GetByIdAsync(matchId);

        if (match == null)
            throw new KeyNotFoundException($"Match with ID {matchId} not found.");
        match.EndTime = DateTime.Now;
        match.Team1Score = team1Score;
        match.Team2Score = team2Score;
        match.PlayerStats = matchStats;
        match.State = MatchState.Finished;
        await matchDataAccess.UpdateAsync(match);
        await matchDataAccess.SaveChangesAsync();
    }


    public async Task JoinMatchAsync(Guid matchId, ApplicationUser player, int teamNumber)
    {
        var match = await matchDataAccess.GetByIdAsync(matchId);

        if (teamNumber == 1)
        {
            if(match.Team1.Players.Count < 2){
                match.Team1.Players.Add(player);
            }
            else
            {
                throw new ArgumentException("Team is full.");
            }
        }
        else if(teamNumber == 2)
        {
            if(match.Team2.Players.Count < 2){
                match.Team2.Players.Add(player);
            }
            else
            {
                throw new ArgumentException("Team is full.");
            }
        }
    }

    public async Task LeaveMatchAsync(Guid matchId, ApplicationUser player, int teamNumber)
    {
        var match = await matchDataAccess.GetByIdAsync(matchId);

        if (teamNumber == 1)
        {
            if(match.Team1.Players.Count < 2){
                match.Team1.Players.Remove(player);
            }
            else
            {
                throw new ArgumentException("Team is full.");
            }
        }
        else if(teamNumber == 2)
        {
            if(match.Team2.Players.Count < 2){
                match.Team2.Players.Remove(player);
            }
            else
            {
                throw new ArgumentException("Team is full.");
            }
        }
        else
        {
            throw new ArgumentException("Invalid team number.");
        }
        await matchDataAccess.UpdateAsync(match);
        await matchDataAccess.SaveChangesAsync();
    }

}