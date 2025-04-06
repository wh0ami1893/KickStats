using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KickStats.Data.Models
{
    public class PlayerMatchStats
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // Foreign key to Match
        public Guid MatchId { get; set; }
        public Match Match { get; set; }

        // Foreign key to ApplicationUser (Player)
        public Guid PlayerId { get; set; }
        public ApplicationUser Player { get; set; }

        // Points scored by the player
        public int Points { get; set; } = 0;
    }
}