
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Worker")]
    public class Worker
    {
        public int Id { get; set; }

        [Column(TypeName = "money")]
        public decimal Fee { get; set; }

        [Required]
        public bool Status { get; set; } = false;

        public List<OrderHistory> OrderHistories { set; get; }

        public List<Workers_Chores> Workers_Chores {set; get;}
        
        public User User { get; set; }

        [ConcurrencyCheck]
        public Guid Version { get; set; }
    }
}