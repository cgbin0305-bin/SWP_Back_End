
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("HouseHoldChores")]
    public class HouseHoldChores
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ChoresName { get; set; }
        [Required]
        public string Description { get; set; }
    }
}