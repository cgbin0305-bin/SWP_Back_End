namespace API.DTOs;

public class ReviewOfUserDto
{
    public int OrderId { get; set; }

    public string ReviewContent { get; set; }

    public int Rate { get; set; }

    public string Email { get; set; }
}