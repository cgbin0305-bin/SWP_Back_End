using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class TrackingWorderDto
{
    [Required]
    public int WorkerId { get; set; }

    [Required]
    public bool IsApprove { get; set; }
}