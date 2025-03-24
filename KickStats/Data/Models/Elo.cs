using System.ComponentModel.DataAnnotations.Schema;

namespace KickStats.Data.Models
{
    public class Elo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public ApplicationUser Player { get; set; }

        public int Score { get; set; }

        public Elo(int score)
        {
            Score = score;
        }

        public Elo()
        {
        }
    }
}