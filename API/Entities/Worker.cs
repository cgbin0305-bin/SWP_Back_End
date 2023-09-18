
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace API.Entities
{
    [Table("Worker")]
    public class Worker
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        // public byte[] PasswordHash { get; set; }
        // public byte[] PasswordSalt { get; set; }
        public string Password { get; set; }
        [Column(TypeName = "money")]
        public decimal Fee { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool Status { get; set; } = false;
        [Required]
        public int Version { get; set; }

        public List<OrderHistory> orderHistories { set; get; }
    }
}