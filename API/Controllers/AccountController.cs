using System.Text;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using API.Helper;
using AutoMapper;
using Errors;
using System.Text.Json;
namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;

        private readonly ISendMailService _sendMailService;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public static string _storeOtpCode;

        public AccountController(IUserRepository userRepository, ITokenService tokenService, ISendMailService sendMailService, IMapper mapper)
        {
            _tokenService = tokenService;
            _sendMailService = sendMailService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenDto>> Register(RegisterDto registerDto)
        {


            // Check if there email or phone is exist
            if (await _userRepository.CheckUserExistAsync(registerDto.Email, registerDto.Phone))
            {
                throw new EmailOrPhoneIsTakenException();
            }
            var user = _mapper.Map<User>(registerDto);

            // add User to DB
            if (await _userRepository.AddUserAsync(user))
            {
                string path = @"MailContent\RegisterSuccessfull.html";
                // set up to send mail 
                var mailContent = new MailContent()
                {
                    Subject = "Dang ky tai khoan thanh cong",
                    Body = ReadFileHelper.ReadFile(path),
                    To = registerDto.Email
                };
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

            var resultPage = MapEntityHelper.MapEntityPaginationAsync(pageString, users);
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

            var resultPage = MapEntityHelper.MapEntityPaginationAsync<UserDto>(pageString, users);

            return Ok(resultPage);
        }

        [HttpPost("admin/add")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UserDto>> CreateNewAccountByAdmin([FromBody] RegisterDto dto)
        {
            if (await _userRepository.CheckUserExistAsync(dto.Email, dto.Phone))
            {
                throw new EmailOrPhoneIsTakenException();
            }
            var user = _mapper.Map<User>(dto);

            // add User to DB
            if (await _userRepository.AddUserAsync(user))
            {
                // return CreatedAtAction(nameof(GetUserForAdminAsync),
                // new {}, _mapper.Map<UserDto>(user));
                return Ok(_mapper.Map<UserDto>(user));
            }
            return BadRequest("Problem adding new account");
        }

        [HttpDelete("admin/delete/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteAccountByAdmin(int id)
        {
            var account = await _userRepository.GetUserEntityByIdAsync(id);

            if (account is null) return NotFound();

            if (account.Role.Equals("user"))
            {
                if (await _userRepository.DeleteUser(account))
                {
                    return Ok();
                }
                return BadRequest("Problem deleting the account");
            }
            else
            {
                return BadRequest("Can not delete the account which has role is " + account.Role);
            }
        }

        [HttpPost("user/add")]
        [Authorize(Roles = "user")]
        public async Task<ActionResult<string>> UpdateUserToWorker([FromBody] WorkerRegisterDto dto) //[FromBody] WorkerRegisterDto dto
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value);
            var user = await _userRepository.GetUserEntityByIdAsync(userId, includeWorker: true);

            if (user is null) return NotFound();

            var list = dto.ChoresList.Select(chore =>
            {
                return new Workers_Chores { WorkerId = userId, ChoreId = chore };
            }).ToList();
            Worker worker = new Worker
            {
                Id = userId,
                Fee = dto.Fee,
                Workers_Chores = list
            };
            user.Role = "worker";
            user.Worker = worker;
            if (await _userRepository.SaveChangeAsync())
            {
                return NoContent();
            }

            return BadRequest("Fail To User Sign Up To Be a Worker");
        }

        [HttpPut("update")]
        [Authorize(Roles = "user, worker")]
        public async Task<ActionResult> UpdateAccountInfo(AccountUpdateDto accountUpdateDto)
        {
            // get account Id base token
            var accountId = int.Parse(User.FindFirst("userId")?.Value);
            // get account
            var userOrWorker = await _userRepository.GetUserEntityByIdAsync(accountId);
            // update info
            if (!string.IsNullOrEmpty(accountUpdateDto.Address))
            {
                userOrWorker.Address = accountUpdateDto.Address;
            }

            if (!string.IsNullOrEmpty(accountUpdateDto.Name))
            {
                userOrWorker.Name = accountUpdateDto.Name;
            }

            userOrWorker.Phone = accountUpdateDto.Phone;

            if (await _userRepository.SaveChangeAsync()) return NoContent();
            return BadRequest("Fail to update account information");
        }

        [HttpGet("get_account")]
        [Authorize(Roles = "user, worker, admin")]
        public async Task<ActionResult<UserDto>> GetAccountOfUser()
        {
            var userId = User.FindFirst("userId")?.Value;

            var account = await _userRepository.GetUserByIdAsync(int.Parse(userId));

            if (account != null) return Ok(account);

            return BadRequest("Account does not exists");
        }
        [HttpPost("send_otp")]
        public async Task<ActionResult> SendOtp([FromBody] JsonElement body)
        {
            var userEmail = JsonSerializer.Serialize(body);
            userEmail = userEmail.Substring(1, userEmail.Length - 2);
            System.Console.WriteLine(userEmail);
            var user = await _userRepository.GetUserEntityByEmailAsync(userEmail);
            if (user == null)
            {
                return BadRequest("The user is not exist!");
            }
            SendAndRetrieveRandomOtpCode sendAndRetrieveRandomOtpCode = new SendAndRetrieveRandomOtpCode();
            string otpCode = sendAndRetrieveRandomOtpCode.GenerateRandomOtpCode();
            sendAndRetrieveRandomOtpCode.StoreOtpCodeForUser(userEmail, otpCode);
            string path = @"MailContent\SendOtpCode.html";
            string html = ReadFileHelper.ReadFile(path).Replace("[OTP_CODE]", otpCode);
            var mailContent = new MailContent()
            {
                Subject = "Otp Code",
                Body = html,
                To = user.Email
            };
            // send mail
            await _sendMailService.SendMailAsync(mailContent);
            // get otp code 
            _storeOtpCode = sendAndRetrieveRandomOtpCode.GetStoredOtpCodeForUser(userEmail);
            return Ok();
        }
        [HttpPost("forgot_password")]
        public async Task<ActionResult> CheckOtp([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            // check otp_code
            if (_storeOtpCode != forgotPasswordDto.OtpCode)
            {
                return BadRequest("Incorrect Otp Code");
            }
            // get user by email
            var user = await _userRepository.GetUserEntityByEmailAsync(forgotPasswordDto.Email);
            //check new password if it duplicate with old password
            // using var hmac = new HMACSHA512(user.PasswordSalt);
            // var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(forgotPasswordDto.NewPassword));
            // for (int i = 0; i < computeHash.Length; i++)
            // {
            // if (computeHash[i] != user.PasswordHash[i])
            // {
            // update password
            var Password = forgotPasswordDto.NewPassword.GetPasswordHash();
            user.PasswordHash = Password.Item1;
            user.PasswordSalt = Password.Item2;
            if (await _userRepository.SaveChangeAsync()) return NoContent();
            return BadRequest("Fail to update password");
            // }
            // continue;
            // }
            // return BadRequest("The new password must not be the same as the old password");
            // }
        }

        [HttpPost("change_password")]
        [Authorize(Roles = "user, worker, admin")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var accountId = int.Parse(User.FindFirst("userId")?.Value);
            var user = await _userRepository.GetUserEntityByIdAsync(accountId);
            if (user is null)
            {
                return BadRequest("User is not existed");
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(changePasswordDto.OldPassword));
            for (int i = 0; i < computeHash.Length; i++)
            {
                if (computeHash[i] != user.PasswordHash[i]) return BadRequest("Invalid old password");
            }
            var passwordHash = changePasswordDto.NewPassword.GetPasswordHash();
            user.PasswordHash = passwordHash.Item1;
            user.PasswordSalt = passwordHash.Item2;
            if (await _userRepository.SaveChangeAsync()) return Ok();
            return BadRequest("Fail to change password");
        }
    }
}
