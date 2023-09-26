using API.Entities;

namespace API.DTOs;

public class WorkerDetail
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Fee { get; set; }
    public string Address { get; set; }
    public int AverageRate { get; set; }
    public int CountOrder {get; set;}
    public List<ReviewDto> Reviews {set; get;}
    public List<HouseHoldChoresDto> Chores { set; get; }
    
}