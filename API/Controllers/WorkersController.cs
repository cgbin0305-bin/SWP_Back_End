using API.DTOs;
using API.Helper;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  public class WorkersController : BaseApiController
  {
    private readonly IWorkerRepository _workerRepository;

    public WorkersController(IWorkerRepository workerRepository)
    {
      _workerRepository = workerRepository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkerDto>> GetWorkerById(int id)
    {
      // get workers based on there id
      var worker = await _workerRepository.GetWorkerByIdAsync(id);

      // check if worker is null
      if (worker is null)
      {
        return BadRequest("Worker does not exist");
      }

      return Ok(worker);
    }

    [HttpGet]
    public async Task<ActionResult<WorkersByPage>> GetWorkerByPage([FromQuery(Name = "page")] string pageString)
    {
      int currentPage;
      float pageSize = 12f;
      var workers = await _workerRepository.GetAllWorkers();

      try
      {
        currentPage = PageHelper.CurrentPage(pageString, workers.Count(), pageSize);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      var listWorkerPage = new List<WorkerPage>();
      foreach (var worker in workers.Skip(currentPage * (int)pageSize).Take((int)pageSize).ToList())
      {
        var workerPage = new WorkerPage
        {
          Id = worker.Id,
          Name = worker.Name,
          Fee = worker.Fee,
          Address = worker.Address,
          AverageRate = worker.AverageRate,
          Chores = worker.Chores
        };
        listWorkerPage.Add(workerPage);
      }

      var resultPage = new WorkersByPage()
      {
        Workers = listWorkerPage,
        CurrentPage = currentPage,
        TotalElements = workers.Count(),
        PageSize = (int)pageSize
      };

      return Ok(resultPage);
    }

    [HttpGet("search")]
    public async Task<ActionResult<WorkersByPage>> SearchWorkers([FromQuery(Name ="keyword")] string keyword,[FromQuery(Name ="page")] string pageString)
    {
      if (string.IsNullOrWhiteSpace(keyword))
      {
        return NoContent();
      }

      int currentPage;
      float pageSize = 12f;
      var workers = await _workerRepository.SearchWorkersAsync(keyword);

      try
      {
        currentPage = PageHelper.CurrentPage(pageString, workers.Count(), pageSize);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      var listWorkerPage = new List<WorkerPage>();
      foreach (var worker in workers.Skip(currentPage * (int)pageSize).Take((int)pageSize).ToList())
      {
        var workerPage = new WorkerPage
        {
          Id = worker.Id,
          Name = worker.Name,
          Fee = worker.Fee,
          Address = worker.Address,
          AverageRate = worker.AverageRate,
          Chores = worker.Chores
        };
        listWorkerPage.Add(workerPage);
      }

      var resultPage = new WorkersByPage()
      {
        Workers = listWorkerPage,
        CurrentPage = currentPage,
        TotalElements = workers.Count(),
        PageSize = (int)pageSize
      };

      return Ok(resultPage);
    }
  }
}
