using KickStats.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace KickStats.Data;

public class ApplicationUser : IdentityUser<Guid>
{
    public ICollection<Team> Teams { get; set; } = new List<Team>(); // This tracks all teams where the user is Player1 or Player2
    public Elo CurrentElo { get; set; }
    public ICollection<EloHistory> EloHistory { get; set; } = new List<EloHistory>();
}