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
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;

        private readonly ISendMailService _sendMailService;
        private readonly IMapper _mapper;
        private readonly IWorkerRepository _workerRepository;
        private readonly IUserRepository _userRepository;

        public AccountController(IUserRepository userRepository, ITokenService tokenService, ISendMailService sendMailService, IMapper mapper, IWorkerRepository workerRepository)
        {
            _tokenService = tokenService;
            _sendMailService = sendMailService;
            _mapper = mapper;
            _workerRepository = workerRepository;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenDto>> Register(RegisterDto registerDto)
        {

            var mailContent = new MailContent();
            // Check if there email or phone is exist
            if (await _workerRepository.CheckWorkerExistAsync(registerDto.Email, registerDto.Phone))
            {
                return BadRequest("This worker is taken by email or phone number");
            }
            var user = _mapper.Map<User>(registerDto);

            // add User to DB
            if (await _userRepository.AddUserAsync(user))
            {
                // set up to send mail 
                mailContent.Subject = "Dang ky tai khoan thanh cong";
                mailContent.Body = $"Cam on {registerDto.Name} da dang ky tai khoan";
                mailContent.To = registerDto.Email;
                // send mail
                await _sendMailService.SendMailAsync(mailContent);
                // return to client worker name and there token
                return new TokenDto
                {
                    Name = user.Name,
                    Token = _tokenService.CreateToken(user)
                };
            }
            return BadRequest("Some Problem in Signing Up");
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto loginDto)
        {
            // check if user email is not exist
            var user = await _userRepository.CheckUserExistAsync(loginDto);
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

        [HttpPut("admin/update")]
        // [Authorize(Roles = "admin")]
        public async Task<ActionResult> UpdateAccountInfo(AccountUpdateDto accountUpdateDto)
        {
            var user = await _userRepository.GetUserEntityByIdAsync(accountUpdateDto.Id);
            if (user is null)
            {
                return NotFound();
            }
            if (!user.Version.Equals(new Guid(accountUpdateDto.Version)))
            {
                return BadRequest("Concurrency conflict detected. Please reload the data.");
            }

            _mapper.Map(accountUpdateDto, user);
            user.Version = Guid.NewGuid();
            if (await _workerRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Fail to update account information");
        }

        [HttpPost("admin/add")]
        // [Authorize(Roles = "admin")]
        public async Task<ActionResult<UserDto>> CreateNewAccountByAdmin([FromBody] RegisterDto dto)
        {
            if (await _workerRepository.CheckWorkerExistAsync(dto.Email, dto.Phone))
            {
                return BadRequest("Email or phone number is taken");
            }
            var user = _mapper.Map<User>(dto);

            // add User to DB
            if (await _userRepository.AddUserAsync(user))
            {
                // return CreatedAtAction(nameof(GetUserForAdminAsync),
                // new {pageString = 0 }, _mapper.Map<UserDto>(user));
                return Ok();
            }
                return BadRequest("Problem adding new account");
            
        }

        [HttpDelete("admin/delete/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteAccountByAdmin(int id)
        {
            var account = await _userRepository.GetUserEntityByIdAsync(id);

            if (account is null) return BadRequest("Concurrency conflict detected. Please reload the data.");

            if (account.Role == "worker")
            {   
                var updateWorker = new WorkerStatusDto
                {
                    WorkerId = account.Id,
                    Status = false,
                    Version = account.Worker.Version.ToString()
                };
                try
                {
                    if (!await _workerRepository.UpdateWorkerStatusAsync(updateWorker))
                    {
                        return BadRequest("Fail to update worker status");
                    }
                    return Ok();
                }
                catch (InvalidOperationException ex)
                {

                    return BadRequest(ex.Message);
                }
            }

            if(await _userRepository.DeleteUser(account)) {
                return Ok();
            }

            return BadRequest("Problem deleteting the account");
        }
    }
}