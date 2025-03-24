using System.ComponentModel.DataAnnotations.Schema;

namespace KickStats.Data.Models
{
    public class Team
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public ICollection<ApplicationUser> Players { get; set; } = new List<ApplicationUser>();
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}