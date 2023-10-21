using API.DTOs;

namespace DTOs;

public class OrderHistoryOfWorkerDto
{
    public int Id { set; get; }
    public string GuestEmail { get; set; }
    public string Date { set; get; }
    public int Rate { set; get; }
    public string GuestName { set; get; }
    public string GuestPhone { set; get; }
    public string GuestAddress { set; get; }
    public ReviewDto Review {set; get;}
}