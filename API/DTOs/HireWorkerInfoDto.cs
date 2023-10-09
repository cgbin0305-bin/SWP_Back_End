
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class HireWorkerInfoDto
    {
        public string GuestAddress { set; get; }
        public string GuestEmail { set; get; }
        public string GuestName { set; get; }
        public string GuestPhone { set; get; }
        [Required]
        public int WorkerId { set; get; }
    }
}