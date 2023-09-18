
using API.Entities;

namespace API.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Fee { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public List<HouseHoldChoresDto> chores { set; get; }
    }
}