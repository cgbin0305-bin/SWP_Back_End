using System.Text;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using API.Helper;
namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly WebContext _context;

        private readonly ITokenService _tokenService;

        private readonly ISendMailService _sendMailService;

        private readonly IUserRepository _userRepository;

        public AccountController(WebContext context, IUserRepository userRepository, ITokenService tokenService, ISendMailService sendMailService)
        {
            _context = context;
            _tokenService = tokenService;
            _sendMailService = sendMailService;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenDto>> Register(RegisterDto registerDto)
        {

            var mailContent = new MailContent();
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


            // set up to send mail 
            mailContent.Subject = "Dang ky tai khoan thanh cong";
            mailContent.Body = $"Cam on {registerDto.Name} da dang ky tai khoan";
            mailContent.To = registerDto.Email;
            // send mail
            await _sendMailService.SendMailAsync(mailContent);
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

        [HttpGet("admin")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<EntityByPage<UserDto>>> GetUserForAdminAsync([FromQuery(Name = "page")] string pageString)
        {
            var users = await _userRepository.GetAllUsersAsync();
            if (users is null)
            {
                return BadRequest("Users is not exist");
            }

            var resultPage = MapEntityHelper.MapEntityPaginationAsync<UserDto>(pageString, users, 12f);
            return Ok(resultPage);
        }


        [HttpGet("admin/search")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<EntityByPage<UserDto>>> SearchUsersByKeyword([FromQuery(Name = "keyword")] string keyword, [FromQuery(Name = "page")] string pageString)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return NotFound();
            }
            var users = await _userRepository.SearchUserAsync(keyword.ToLower());

            var resultPage = MapEntityHelper.MapEntityPaginationAsync<UserDto>(pageString, users, 12f);

            return Ok(resultPage);
        }
    }
}