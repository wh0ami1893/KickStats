using System.ComponentModel.DataAnnotations.Schema;

namespace KickStats.Data.Models
{
    public class PlayTable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<Match> Matches { get; set; } = new List<Match>();

        public Match CurrentMatch { get; set; }
    }
}