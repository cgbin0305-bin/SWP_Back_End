using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class WorkerRegisterDto
    {
        [Required]
        public List<int> ChoresList { get; set; }

        public decimal Fee { get; set; } = 25000;
    }
}