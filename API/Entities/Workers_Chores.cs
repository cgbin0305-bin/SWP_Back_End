using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Workers_Chores")]
    public class Workers_Chores
    {
        public int WorkerId { get; set; }
        [ForeignKey("WorkerId")]
        public Worker Worker { get; set; }

        public int ChoreId { get; set; }
        [ForeignKey("ChoreId")]
        public HouseHoldChores Chore { get; set; }
        [ConcurrencyCheck]
        public Guid Version { get; set; }
    }
}