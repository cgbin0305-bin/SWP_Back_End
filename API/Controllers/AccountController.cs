using System.Text;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly WebContext _context;

        private readonly ITokenService _tokenService;
        public AccountController(WebContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenDto>> Register(RegisterDto registerDto)
        {
            // Check if there email or phone is exist
            if (await WorkerExist(registerDto.Email, registerDto.Phone))
            {
                return BadRequest("This worker is taken by email or phone number");
            }
            // set all prop in registerDto into user and worker table
            using var hmac = new HMACSHA512();
            var user = new User()
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                Role = "User".ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
                Address = registerDto.Address
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            // return to client worker name and there token
            return new TokenDto
            {
                Name = user.Name,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto loginDto)
        {
            // check if user email is not exist
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Email.Equals(loginDto.Email));
            if (user is null) return Unauthorized("Invalid Email");
            //check password by reverse to what we did before computed hash
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computeHash.Length; i++)
            {
                if (computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }
            // return to client worker name and token 
            return new TokenDto
            {
                Name = user.Name,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> WorkerExist(string Email, string Phone)
        {
            return await _context.Users.AnyAsync(u => u.Email == Email.ToLower() || u.Phone == Phone);
        }
    }
}