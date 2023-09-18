
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class LoginDto
    {
        [Required]
        public string Email { set; get; }
        [Required]

        public string Password { set; get; }
    }
}