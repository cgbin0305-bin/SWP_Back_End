using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<WorkerDto>> Register(RegisterDto registerDto)
        {
            // Check if there email or phone is exist
            if (await WorkerExist(registerDto.Email, registerDto.Phone))
            {
                return BadRequest("This worker is taken by email or phone number");
            }
            // set all prop in registerDto into worker then add into Workers table
            var worker = new Worker()
            {
                Name = registerDto.Name,
                Password = registerDto.Password,
                Email = registerDto.Email.ToLower(),
                Fee = 0,
                IsAdmin = false,
                Status = false,
                Phone = registerDto.Phone,
                Version = 0,
            };

            _context.Workers.Add(worker);
            await _context.SaveChangesAsync();
            // add chores id of worker , and worker id into table Workers_Chores table
            foreach (var choresId in registerDto.RoleChores)
            {
                _context.Workers_Chores.Add(new Workers_Chores
                {
                    WorkerId = worker.Id,
                    ChoreId = choresId
                });
            }

            await _context.SaveChangesAsync();
            // return to client worker name and there token
            return new WorkerDto
            {
                Name = worker.Name,
                Token = _tokenService.CreateToken(worker)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<WorkerDto>> Login(LoginDto loginDto)
        {   
            // check if worker email is not exist
            var worker = await _context.Workers.FirstOrDefaultAsync(worker => worker.Email.Equals(loginDto.Email));
            if (worker is null) return Unauthorized("Invalid Email");
            // check if worker password is not match
            if (!worker.Password.Equals(loginDto.Password)) return Unauthorized("Invalid Password");
            // return to client worker name and token 
            return new WorkerDto
            {
                Name = worker.Name,
                Token = _tokenService.CreateToken(worker)
            };
        }

        private async Task<bool> WorkerExist(string Email, string Phone)
        {
            return await _context.Workers.AnyAsync(w => w.Email == Email.ToLower() || w.Phone == Phone);
        }
    }
}