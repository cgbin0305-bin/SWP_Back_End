namespace API.DTOs;

public class WorkerUpdateDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Fee { get; set; }

    public string Address { get; set; }

    public List<int> Chores { get; set; }

    public string Version {get; set;}
}