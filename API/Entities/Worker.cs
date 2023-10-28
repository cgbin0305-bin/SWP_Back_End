
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
        public decimal Fee { get; set; } = 25000;

        public bool Status { get; set; } = false;

        public string PhotoUrl { get; set; }

        public string PublicId { get; set; }

        public List<OrderHistory> OrderHistories { set; get; }

        public List<Workers_Chores> Workers_Chores { set; get; }

        public User User { get; set; }

        [ConcurrencyCheck]
        public Guid Version { get; set; }
    }
}