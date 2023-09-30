namespace API.DTOs;

public class ReviewDto
{
    public int Id { get; set; }

    public string GuestName {set; get;}

    public string Content { get; set; }

    public string Date { get; set; }

    public int Rate { get; set; }
}