
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDto> GetUserByIdAsync(int userId);

        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<IEnumerable<UserDto>> SearchUserAsync(string keyword);
        Task<bool> AddUserAsync(User user);
        Task<bool> SaveChangeAsync();
        Task<User> CheckUserExistAsync(LoginDto dto);
        Task<User> GetUserEntityByIdAsync(int Id);

        Task<bool> DeleteUser(User user);
    }
}