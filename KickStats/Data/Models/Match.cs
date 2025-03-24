using System.ComponentModel.DataAnnotations.Schema;

namespace KickStats.Data.Models
{
    public class Match
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime MatchDate { get; set; }
        public DateTime EndTime { get; set; }

        public Guid PlayTableId { get; set; }
        public PlayTable PlayTable { get; set; } = null!;

        public Guid Team1Id { get; set; }
        public Team Team1 { get; set; }

        public Guid Team2Id { get; set; }
        public Team Team2 { get; set; }

        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
    }
}