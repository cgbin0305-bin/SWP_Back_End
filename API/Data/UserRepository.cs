using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly WebContext _context;
        private readonly IMapper _mapper;

        public UserRepository(WebContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            return await _context.Users
            .Where(u => u.Id == userId)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        }

        public async Task<IEnumerable<UserDto>> SearchUserAsync(string keyword)
        {
            var users = await _context.Users
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .Where(x => x.Email.ToLower().Contains(keyword) || x.Address.ToLower().Contains(keyword) || x.Name.ToLower().Contains(keyword) || x.Phone.ToLower().Contains(keyword) || x.Role.ToLower().Contains(keyword))
            .ToListAsync();

            return users;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            return await SaveChangeAsync();
        }
        public async Task<bool> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<User> CheckUserExistAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Email.Equals(dto.Email));
            return user;
        }

        public async Task<User> GetUserEntityByIdAsync(int Id)
        {
            return await _context.Users
            .Where(x => x.Id == Id)
            .Include(x => x.Worker)
            .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteUser(User user) {
            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}