namespace API.DTOs;

public class OrderHistoryDto
{
        public int Id { set; get; }
        public string GuestEmail { get; set; }
        public string Date { set; get; }
        public string GuestName { set; get; }
        public string GuestPhone { set; get; }
        public string GuestAddress { set; get; }
        public int WorkerId { get; set; }

        public string WorkerName { get; set; }
}