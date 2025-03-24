using System.ComponentModel.DataAnnotations.Schema;

namespace KickStats.Data.Models
{
    public class EloHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int Score { get; set; }

        public DateTime Timestamp { get; set; }

        public Guid PlayerId { get; set; }
        public ApplicationUser Player { get; set; }
    }
}