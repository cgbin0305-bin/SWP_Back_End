
using API.Entities;

namespace API.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Fee { get; set; }
        public string Address { get; set; }
        public int AverageRate { get; set; }
        public List<HouseHoldChoresDto> Chores { set; get; }
    }
}