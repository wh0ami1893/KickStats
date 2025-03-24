using KickStats.Data;
using KickStats.Data.Models;
using KickStats.Repositories.Interfaces;
using KickStats.Services.Interfaces;

namespace KickStats.Services.Implementations;

public class MatchService(IMatchDataAccess dataAccess) : IMatchService
{
    public async Task<Match> CreateMatchAsync(Guid playTableId, DateTime matchDate)
    {
        var match = new Match()
        {
            PlayTableId = playTableId,
            Team1 = new Team(),
            Team2 = new Team(),
            State = MatchState.Open
        };
        await dataAccess.AddAsync(match);
        await dataAccess.SaveChangesAsync();
        return match;
    }

    public async Task<Match> GetMatchByIdAsync(Guid matchId)
    {
        var match = await dataAccess.GetByIdAsync(matchId);
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
       await dataAccess.UpdateAsync(match);
       await dataAccess.SaveChangesAsync();
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

        await dataAccess.UpdateAsync(match);
        await dataAccess.SaveChangesAsync();
    }

    public async Task CloseMatchAsync(Guid matchId, int team1Score, int team2Score)
    {
        var match = await dataAccess.GetByIdAsync(matchId);

        if (match == null)
            throw new KeyNotFoundException($"Match with ID {matchId} not found.");
        match.EndTime = DateTime.Now;
        match.Team1Score = team1Score;
        match.Team2Score = team2Score;
        await dataAccess.UpdateAsync(match);
        await dataAccess.SaveChangesAsync();
    }


    public async Task JoinMatchAsync(Guid matchId, ApplicationUser player, int teamNumber)
    {
        var match = await dataAccess.GetByIdAsync(matchId);

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
        var match = await dataAccess.GetByIdAsync(matchId);

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
        await dataAccess.UpdateAsync(match);
        await dataAccess.SaveChangesAsync();
    }

}