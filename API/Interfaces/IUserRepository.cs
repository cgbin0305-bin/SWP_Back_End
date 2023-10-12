
using API.DTOs;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDto> GetUserByIdAsync(int userId);

        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<IEnumerable<UserDto>> SearchUserAsync(string keyword);
    }
}