
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class WorkerStatusDto
    {
        [Required]
        public int WorkerId { get; set; }
        [Required]
        public bool Status { get; set; }
    }
}