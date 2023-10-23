using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class AccountUpdateDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        [Required]
        public string Version { set; get; }
    }
}