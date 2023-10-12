namespace API.DTOs;

public class EntityByPage<T>
{
        public IEnumerable<T> List { set; get; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalElements { get; set; }
}