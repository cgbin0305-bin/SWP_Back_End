
namespace API.DTOs
{
    public class WorkersByPage
    {
        public List<WorkerPage> Workers { set; get; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalElements { get; set; }
    }
}