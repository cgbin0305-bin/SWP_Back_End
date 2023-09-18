

using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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
                }
                else
                {
                    page = 0;
                }
                currentPage = page + 1;
            }

            if (page < 0 || page > pageCount)
            {
                return BadRequest("The result were not found");
            }

            List<AccountDto> WorkerList = new List<AccountDto>();

            var qr = (from a in _context.Workers_Chores
                      where a.Worker.Status == true
                      group a by a.WorkerId into gr
                      select new
                      {
                          Worker = _context.Workers.FirstOrDefault(w => w.Id == gr.Key),
                          ListChores = gr.ToList()
                      }).Skip(page * (int)pageResults)
                      .Take((int)pageResults)
                      .ToList();


            foreach (var item in qr)
            {
                List<HouseHoldChoresDto> houseHoldChoresList = new List<HouseHoldChoresDto>();
                foreach (var Chore in item.ListChores)
                {
                    var entry = _context.Entry(Chore);
                    await entry.Reference(x => x.Chore).LoadAsync();

                    HouseHoldChoresDto dto = new HouseHoldChoresDto()
                    {
                        Id = Chore.ChoreId,
                        Name = Chore.Chore.ChoresName,
                        Description = Chore.Chore.Description
                    };
                    houseHoldChoresList.Add(dto);
                }

                AccountDto accountDto = new AccountDto()
                {
                    Id = item.Worker.Id,
                    Name = item.Worker.Name,
                    Email = item.Worker.Email,
                    Fee = item.Worker.Fee,
                    Phone = item.Worker.Phone,
                    chores = houseHoldChoresList
                };
                WorkerList.Add(accountDto);
            }
            var Response = new AccountResponseDto()
            {
                Accounts = WorkerList,
                CurrentPage = currentPage,
                Elements = WorkerList.Count,
                Pages = (int)pageCount
            };
            return Ok(Response);
        }


    }
}
