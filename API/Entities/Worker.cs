
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

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
        [ConcurrencyCheck]
        public Guid Version { get; set; }

        public List<OrderHistory> orderHistories { set; get; }
        public User User { get; set; }
    }
}