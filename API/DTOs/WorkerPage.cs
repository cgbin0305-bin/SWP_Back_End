
using API.Entities;

namespace API.DTOs
{
    public class WorkerPage
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Fee { get; set; }

        public bool Status { get; set; }

        public string Address { get; set; }

        public string PhotoUrl { get; set; }
        public string WorkingState { get; set; }

        public int AverageRate { get; set; }

        public IEnumerable<HouseHoldChoresDto> Chores { set; get; }

        public Guid Version { get; set; }
    }
}