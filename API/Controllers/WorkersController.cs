using API.DTOs;
using API.Entities;
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
            // get workers based on there id
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
            // each page site have 12 elements
            var pageResults = 12f;
            // total of elements in the list (which is have status = true)
            var totalElements = _context.Workers.Where(x => x.Status == true);
            // total of page count
            var pageCount = Math.Ceiling(totalElements.Count() / pageResults);
            try
            {
                // check string if empty => page = 0
                if (string.IsNullOrEmpty(pageString))
                {
                    page = 0;
                }
                else
                {   
                    // try to parse string into int
                    page = Int32.Parse(pageString);
                }
                currentPage = page;

            }
            catch (System.Exception)
            {
                // user enter double type => floor it 
                // Replace ',' to '.' Ex: instead of enter 1.2 but user enter 1,2 
                string input = pageString.Replace(',', '.');
                decimal outputDecimal;
                //  Parse string into decimal and floor it
                if (decimal.TryParse(input, out outputDecimal))
                {
                    page = (int)Math.Floor(outputDecimal);
                }
                else
                {
                    // if user enter string text => set page = 0
                    page = 0;
                }
                // set current page
                currentPage = page;
            }
            // check if the page is out of range 0 || page Count 
            if (page < 0 || page > pageCount)
            {
                return BadRequest("The result were not found");
            }

            List<AccountDto> WorkerList = new List<AccountDto>();

            // Get all workers and chores based on Workers_Chores table
            var qr = (from a in _context.Workers_Chores
                      where a.Worker.Status == true
                      group a by a.WorkerId into gr
                      select new
                      {
                        //  Get All workers and list of chores
                          Worker = _context.Workers.FirstOrDefault(w => w.Id == gr.Key),
                          ChoresList = gr.ToList()
                      }).Skip(page * (int)pageResults)
                      .Take((int)pageResults)
                      .ToList();

            foreach (var item in qr)
            {
                List<HouseHoldChoresDto> houseHoldChoresList = new List<HouseHoldChoresDto>();
                foreach (var Chore in item.ChoresList)
                {
                    // Get all chores by using Entry
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
            // add to account response 
            var Response = new AccountResponseDto()
            {
                Accounts = WorkerList,
                CurrentPage = currentPage,
                TotalElements = totalElements.Count(),
                PageSize = (int)pageResults
            };
            return Ok(Response);
        }


    }
}
