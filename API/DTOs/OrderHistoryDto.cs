namespace API.DTOs;

public class OrderHistoryDto
{
        public int Id { set; get; }
        public string GuestEmail { get; set; }
        public DateTime Date { set; get; }
        public string GuestName { set; get; }
        public string GuestPhone { set; get; }
        public string GuestAddress { set; get; }
        public ReviewDto Review { get; set; }
}