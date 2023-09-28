using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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
    public async Task<ActionResult<WorkerDetail>> GetWorkerById(int id)
    {
      // get workers based on there id
      var worker = await _context.Workers.Where(x => x.Status == true).FirstOrDefaultAsync(x => x.Id == id);

      // check if worker is null
      if (worker is null)
      {
        return BadRequest("Worker does not exist");
      }
      // explicit Load User
      var entry = _context.Entry(worker);
      await entry.Reference(e => e.User).LoadAsync();
      var user = worker.User;

      // get all chores that worker do
      var chores = (from c in _context.HouseHoldChores
                    join wc in _context.Workers_Chores on c.Id equals wc.ChoreId
                    where wc.WorkerId == worker.Id
                    select c).ToList();

      List<HouseHoldChoresDto> list = new List<HouseHoldChoresDto>();
      // add chores to list
      foreach (var item in chores)
      {
        HouseHoldChoresDto choresDto = new HouseHoldChoresDto
        {
          Id = item.Id,
          Name = item.Name,
          Description = item.Description
        };
        list.Add(choresDto);
      }

      // get OrderHistory
      var reviews = _context.OrderHistories.Where(o => o.WorkerId == id)
          .Include(o => o.Review)
          .ToList();

      // get a review of this worker
      List<ReviewDto> listReviews = new List<ReviewDto>();
      // variable to caculate average rate  
      double sum = 0;
      int count = reviews.Count();
      foreach (var item in reviews)
      {
        var reviewDto = new ReviewDto
        {
          GuestName = item.GuestName,
          Date = string.Format("{0:yyyy-MM-dd}", item.Date),
          Content = item.Review.Content,
          Rate = item.Review.Rate
        };
        sum += reviewDto.Rate;
        listReviews.Add(reviewDto);
      }


      WorkerDetail workerDetail = new WorkerDetail
      {
        Id = worker.Id,
        Fee = worker.Fee,
        Name = user.Name,
        Address = user.Address,
        AverageRate = count > 0 ? (int)Math.Round(sum / count) : 0,
        CountOrder = count,
        Reviews = listReviews,
        Chores = list
      };
      return Ok(workerDetail);
    }

    [HttpGet]
    public async Task<ActionResult<AccountResponseDto>> GetWorkerByPage([FromQuery(Name = "page")] string pageString)
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
        var entryUser = _context.Entry(item.Worker);
        entryUser.Reference(x => x.User).Load();
        foreach (var Chore in item.ChoresList)
        {
          // Get all chores by using Entry
          var entry = _context.Entry(Chore);
          await entry.Reference(x => x.Chore).LoadAsync();

          HouseHoldChoresDto dto = new HouseHoldChoresDto()
          {
            Id = Chore.ChoreId,
            Name = Chore.Chore.Name,
            Description = Chore.Chore.Description
          };
          houseHoldChoresList.Add(dto);
        }

        AccountDto accountDto = new AccountDto()
        {
          Id = item.Worker.Id,
          Name = item.Worker.User.Name,
          Fee = item.Worker.Fee,
          Address = item.Worker.User.Address,
          AverageRate = 0,
          Chores = houseHoldChoresList
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
