
namespace API.DTOs
{
    public class AccountResponseDto
    {
        public List<AccountDto> Accounts { set; get; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalElements { get; set; }
    }
}