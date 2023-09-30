namespace API.DTOs;

public class WorkerDto
{
    public int Id { get; set; }

    public decimal Fee { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public int CountOrder { set; get; }

    public int AverageRate { set; get; }

    public List<ReviewDto> Reviews { set; get; }

    public IEnumerable<HouseHoldChoresDto> Chores {set; get;}

}