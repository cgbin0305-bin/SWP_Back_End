namespace API.DTOs
{
    public class OrderHistoryByPage
    {
        public List<OrderHistoryDto> OrderHistories { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalElements { get; set; }
    }
}