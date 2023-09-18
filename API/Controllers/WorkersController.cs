
using System.ComponentModel;
using System.Globalization;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class WorkersController : BaseApiController
    {
        private readonly WebContext _context;
        public WorkersController(WebContext context)
        {
            _context = context;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> GetWorkerById(int id)
        {
            var worker = await _context.Workers.Where(x => x.Status == true).FirstOrDefaultAsync(x => x.Id == id);
            AccountDto dto = new AccountDto()
            {
                Id = worker.Id,
                Name = worker.Name,
                Email = worker.Email,
                Fee = worker.Fee,
                Phone = worker.Phone
            };
            return dto;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetWorkerByPage([FromQuery(Name = "page")] string pageString)
        {
            int page = 0;
            int currentPage = 0;
            var pageResults = 12f;
            var pageCount = Math.Ceiling(_context.Workers.Count() / pageResults);
            try
            {
                if (string.IsNullOrEmpty(pageString))
                {
                    page = 0;
                }
                else
                {
                    page = Int32.Parse(pageString);
                }
                currentPage = page + 1;

            }
            catch (System.Exception)
            {
                string input = pageString.Replace(',', '.');
                decimal outputDecimal;
                if (decimal.TryParse(input, out outputDecimal))
                {
                    page = (int)Math.Floor(outputDecimal);
                    currentPage = page + 1;
                }
                else
                {
                    page = 0;
                }
            }

            if (page < 0 || page > pageCount)
            {
                return BadRequest("The result were not found");
            }

            List<AccountDto> list = new List<AccountDto>();
            List<HouseHoldChoresDto> listChores = new List<HouseHoldChoresDto>();
            var workers = await _context.Workers.Where(x => x.Status == true)
            .Skip((page) * (int)pageResults)
            .Take((int)pageResults)
            .ToListAsync();
            foreach (var worker in workers)
            {
                AccountDto dto = new AccountDto()
                {
                    Id = worker.Id,
                    Name = worker.Name,
                    Email = worker.Email,
                    Fee = worker.Fee,
                    Phone = worker.Phone
                };
                list.Add(dto);
            }
            var Response = new AccountResponseDto()
            {
                Accounts = list,
                CurrentPage = currentPage,
                Elements = list.Count,
                Pages = (int)pageCount
            };
            return Ok(Response);
        }
    }
}