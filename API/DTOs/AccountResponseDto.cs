
namespace API.DTOs
{
    public class AccountResponseDto
    {
        public List<AccountDto> Accounts { set; get; }
        public int CurrentPage { get; set; }
        public int Pages { get; set; } // Total of page
        public int Elements { get; set; }

    }
}