
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int Rate { get; set; }
        public OrderHistory OrderHistory { get; set; }
        [ConcurrencyCheck]
        public Guid Version { get; set; }
    }
}