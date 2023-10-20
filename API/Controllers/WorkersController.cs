using System.Runtime.InteropServices.ComTypes;
using System.Net;
using API.Data;
using API.DTOs;
using API.Helper;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using API.Entities;

namespace API.Controllers
{
  public class WorkersController : BaseApiController
  {
    private readonly IWorkerRepository _workerRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public WorkersController(IWorkerRepository workerRepository, IMapper mapper, IUserRepository userRepository)
    {
      _workerRepository = workerRepository;
      _mapper = mapper;
      _userRepository = userRepository;
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
    public async Task<ActionResult<EntityByPage<WorkerPage>>> GetWorkerByPage([FromQuery(Name = "page")] string pageString)
    {

      var workers = await _workerRepository.GetAllWorkersAsync();
      if (workers is null)
      {
        return BadRequest("Worker is not exist");
      }

      var workerPages = workers.Select(x => _mapper.Map<WorkerPage>(x));
      var resultPage = MapEntityHelper.MapEntityPaginationAsync(pageString, workerPages);

      return Ok(resultPage);
    }

    [HttpGet("search")]
    public async Task<ActionResult<EntityByPage<WorkerPage>>> SearchWorkers([FromQuery(Name = "keyword")] string keyword, [FromQuery(Name = "page")] string pageString)
    {
      if (string.IsNullOrWhiteSpace(keyword))
      {
        return NotFound();
      }
      var workers = await _workerRepository.SearchWorkersAsync(keyword.ToLower());

      var workerPages = workers.Select(x => _mapper.Map<WorkerPage>(x));
      var resultPage = MapEntityHelper.MapEntityPaginationAsync(pageString, workerPages);

      return Ok(resultPage);
    }

    [HttpGet("admin")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<EntityByPage<WorkerPage>>> GetWorkerForAdminByPage([FromQuery(Name = "page")] string pageString)
    {
      var workers = await _workerRepository.GetAllWorkersForAdminAsync();
      if (workers is null)
      {
        return BadRequest("Worker is not exist");
      }

      var workerPages = workers.Select(x => _mapper.Map<WorkerPage>(x));
      var resultPage = MapEntityHelper.MapEntityPaginationAsync(pageString, workerPages);
      return Ok(resultPage);
    }

    [HttpPost("admin/status")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> UpdateWorkerStatus(WorkerStatusDto dto)
    {
      try
      {
        var checkUpdateStatusIsSuccessOrFail = await _workerRepository.UpdateWorkerStatusAsync(dto);
        if (!checkUpdateStatusIsSuccessOrFail)
        {
          return BadRequest("Fail to update worker status");
        }
        return NoContent();
      }
      catch (InvalidOperationException ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("admin/search")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<EntityByPage<WorkerPage>>> SearchWorkersByAdmin([FromQuery(Name = "keyword")] string keyword, [FromQuery(Name = "page")] string pageString)
    {
      if (string.IsNullOrWhiteSpace(keyword))
      {
        return NotFound();
      }
      var workers = await _workerRepository.SearchWorkersByAdminAsync(keyword.ToLower());
      var workerPages = workers.Select(x => _mapper.Map<WorkerPage>(x));
      var resultPage = MapEntityHelper.MapEntityPaginationAsync(pageString, workerPages);
      return Ok(resultPage);
    }

    [HttpPut("admin/update")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> UpdateWorkerInfo(WorkerUpdateDto workerUpdateDto)
    {
      var worker = await _workerRepository.GetWorkerEntityByIdAsync(workerUpdateDto.Id);

      if (worker == null) return NotFound();

      if (!worker.Version.Equals(new Guid(workerUpdateDto.Version)))
      {
        return BadRequest("Concurrency conflict detected. Please reload the data.");
      }

      _mapper.Map(workerUpdateDto, worker);
      worker.Version = Guid.NewGuid(); // update the new version 

      if (await _workerRepository.SaveAllAsync()) return NoContent();

      return BadRequest("Fail to update worker information");

    }


  }
}
