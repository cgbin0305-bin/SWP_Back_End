using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Name { set; get; }

        [Required]

        public string Password { set; get; }

        [Required]
        public string Email { set; get; }

        [Required]
        public string Phone { set; get; }

        [Required]
        public string Address { set; get; }

        [Required]
        public List<int> RoleChores { set; get; }
    }
}