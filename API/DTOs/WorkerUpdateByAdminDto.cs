using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class WorkerUpdateByAdminDto
{
    [Required]
    public int Id { get; set; }
    [Required]

    public string Name { get; set; }
    [Required]

    public decimal Fee { get; set; }
    [Required]

    public string Address { get; set; }
    [Required]

    public List<int> Chores { get; set; }
    [Required]

    public string Version { get; set; }
}