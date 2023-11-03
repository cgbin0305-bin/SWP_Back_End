using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class TrackingWorker
    {
        public int WorkerId { get; set; }
        [ForeignKey("WorkerId")]
        public Worker Worker { get; set; }

        public int ChoreId { get; set; }
        [ForeignKey("ChoreId")]
        public HouseHoldChores Chore { get; set; }
        [Column(TypeName = "money")]
        public decimal Fee { get; set; }
    }
}