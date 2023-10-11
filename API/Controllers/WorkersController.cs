using System.Runtime.InteropServices.ComTypes;
using System.Net;
using API.Data;
using API.DTOs;
using API.Helper;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

      var workers = await _workerRepository.GetAllWorkersAsync();
      if (workers is null)
      {
        return BadRequest("Worker is not exist");
      }

      var resultPage = SearchWorkerHelper.MapWorkerPaginationAsync(pageString, workers);

      return Ok(resultPage);
    }

    [HttpGet("search")]
    public async Task<ActionResult<WorkersByPage>> SearchWorkers([FromQuery(Name = "keyword")] string keyword, [FromQuery(Name = "page")] string pageString)
    {
      if (string.IsNullOrWhiteSpace(keyword))
      {
        return NoContent();
      }
      var workers = await _workerRepository.SearchWorkersAsync(keyword.ToLower());

      var resultPage = SearchWorkerHelper.MapWorkerPaginationAsync(pageString, workers);

      return Ok(resultPage);
    }

    [HttpGet("admin")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<WorkersByPage>> GetWorkerForAdminByPage([FromQuery(Name = "page")] string pageString)
    {

      var workers = await _workerRepository.GetAllWorkersForAdminAsync();
      if (workers is null)
      {
        return BadRequest("Worker is not exist");
      }

      var resultPage = SearchWorkerHelper.MapWorkerPaginationAsync(pageString, workers);
      return Ok(resultPage);
    }
    [HttpPost("admin/status")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> UpdateWorkerStatus(WorkerStatusDto dto)
    {
      var checkUpdateStatusIsSuccessOrFail = await _workerRepository.UpdateWorkerStatusAsync(dto);
      if (!checkUpdateStatusIsSuccessOrFail)
      {
        return BadRequest("Fail to update worker status");
      }
      return NoContent();
    }

    [HttpGet("admin/search")]
    [Authorize(Roles = "admin")]

    public async Task<ActionResult<WorkersByPage>> SearchWorkersByAdmin([FromQuery(Name = "keyword")] string keyword, [FromQuery(Name = "page")] string pageString)
    {
      if (string.IsNullOrWhiteSpace(keyword))
      {
        return NoContent();
      }
      var workers = await _workerRepository.SearchWorkersAsync(keyword.ToLower());

      var resultPage = SearchWorkerHelper.MapWorkerPaginationAsync(pageString, workers);

      return Ok(resultPage);
    }
  }
}
