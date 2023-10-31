namespace API.DTOs;

public class OrderHistoryOfUserDto
{
        public int Id { set; get; }
        public DateTime Date { set; get; }
        
        public int WorkerId { set; get; }

        public string WorkerName { set; get; }

        public ReviewDto Review { set; get; } 
}