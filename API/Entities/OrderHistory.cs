
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("OrderHistory")]
    public class OrderHistory
    {
        [Key]
        public int Id { set; get; }
        [Required]
        public string GuestEmail { get; set; }
        [Required]
        public DateTime Date { set; get; }
        [Required]
        public string GuestName { set; get; }
        [Required]
        public string GuestPhone { set; get; }
        [Required]
        public string GuestAddress { set; get; }
        public int WorkerId { set; get; }

        [ForeignKey("WorkerId")]
        [InverseProperty("orderHistories")]
        [Required]
        public Worker Worker { set; get; }
        public Review Review { get; set; }
        [ConcurrencyCheck]
        public Guid Version { get; set; }

    }
}