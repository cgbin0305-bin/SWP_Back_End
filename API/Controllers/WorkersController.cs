using API.DTOs;
using API.Helper;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DTOs;
using Org.BouncyCastle.Asn1.Esf;
using API.Services;
using CloudinaryDotNet.Actions;
using API.Entities;

namespace API.Controllers
{
  public class WorkersController : BaseApiController
  {
    private readonly IWorkerRepository _workerRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IPhotoService _photoService;
    private readonly IOrderHistoryRepository _orderHistoryRepository;
    private readonly ISendMailService _sendMailService;

    public WorkersController(IWorkerRepository workerRepository, IMapper mapper, IUserRepository userRepository, IPhotoService photoService, IOrderHistoryRepository orderHistoryRepository, ISendMailService sendMailService)
    {
      _sendMailService = sendMailService;
      _workerRepository = workerRepository;
      _mapper = mapper;
      _userRepository = userRepository;
      _photoService = photoService;
      _orderHistoryRepository = orderHistoryRepository;
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

    [HttpGet("info")]
    public async Task<ActionResult<WorkerDto>> GetWorkerInfoById()
    {
      var userId = User.FindFirst("userId")?.Value;
      // get workers based on there id
      var worker = await _workerRepository.GetWorkerEntityByIdAsync(int.Parse(userId), false, true, true);

      // check if worker is null
      if (worker is null)
      {
        return BadRequest("Worker does not exist");
      }

      var dto = _mapper.Map<WorkerDto>(worker);
      var result = _orderHistoryRepository.CountOrderAndRateOfWorker(worker.Id);

      dto.CountOrder = result.Item1;
      dto.AverageRate = result.Item2;

      return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<EntityByPage<WorkerPage>>> GetWorkerByPage([FromQuery(Name = "page")] string pageString)
    {
      var userId = User.FindFirst("userId")?.Value;
      string address = "";

      if (!string.IsNullOrEmpty(userId))
      {
        var user = await _userRepository.GetUserEntityByIdAsync(int.Parse(userId));
        address = user.Address;
      }

      var workers = await _workerRepository.GetAllWorkersAsync(address);
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
    public async Task<ActionResult> UpdateWorkerInfoByAdmin(WorkerUpdateByAdminDto workerUpdateByAdminDto)
    {
      var worker = await _workerRepository.GetWorkerEntityByIdAsync(workerUpdateByAdminDto.Id, true, true, true);

      if (worker == null) return NotFound();

      if (!worker.Version.Equals(new Guid(workerUpdateByAdminDto.Version)))
      {
        return BadRequest("Concurrency conflict detected. Please reload the data.");
      }

      _mapper.Map(workerUpdateByAdminDto, worker);
      worker.Version = Guid.NewGuid(); // update the new version 

      if (await _workerRepository.SaveAllAsync()) return NoContent();

      return BadRequest("Fail to update worker information");
    }

    [HttpGet("orderhistories")]
    [Authorize(Roles = "worker")]
    public async Task<ActionResult<IEnumerable<OrderHistoryOfWorkerDto>>> GetOrderHistoryForUser()
    {

      var userId = int.Parse(User.FindFirst("userId")?.Value);

      var worker = await _workerRepository.GetWorkerEntityByIdAsync(userId, includeOrderHistories: true);

      if (worker is null) return NotFound();

      var orderhistories = worker.OrderHistories;

      if (orderhistories is null || orderhistories.Count == 0) return BadRequest("This worker has never been scheduled");

      var result = orderhistories.Select(x => _mapper.Map<OrderHistoryOfWorkerDto>(x))
        .OrderByDescending(x => x.Id)
        .ToList();

      return Ok(result);
    }

    [HttpPost("upload-photo")]
    [Authorize(Roles = "worker, user")]
    public async Task<ActionResult<WorkerDto>> UploadPhotoForWorker(IFormFile file)
    {
      var userId = User.FindFirst("userId")?.Value;
      var worker = await _workerRepository.GetWorkerEntityByIdAsync(int.Parse(userId));

      if (worker is null) return NotFound();

      if (!string.IsNullOrEmpty(worker.PhotoUrl))
      {
        var deletionResult = await _photoService.DeletePhotoAsync(worker.PublicId);
        if (deletionResult.Error != null) return BadRequest(deletionResult.Error.Message);
      }

      var result = await _photoService.AddPhotoAsync(file);

      if (result.Error != null) return BadRequest(result.Error.Message);

      worker.PhotoUrl = result.SecureUrl.AbsoluteUri;
      worker.PublicId = result.PublicId;

      if (await _workerRepository.SaveAllAsync())
      {
        return Ok("Upload a photo successfully");
      }

      return BadRequest("Problem uploading photo");
    }

    [HttpPut("order/{orderId}")]
    [Authorize(Roles = "worker")]
    public async Task<ActionResult> ApproveOrFinishOrderOfWorker(int orderId)
    {
      var userId = User.FindFirst("userId")?.Value;
      var worker = await _workerRepository.GetWorkerEntityByIdAsync(int.Parse(userId));

      if (worker is null) return NotFound();

      var orderOfWorker = await _orderHistoryRepository.GetOrderHistoryAsync(orderId);
      if (orderOfWorker is null) return NotFound();

      if (orderOfWorker.WorkerId != worker.Id) return BadRequest("You are not a worker to serve this order");
      if (orderOfWorker.Status.Equals("finished") || orderOfWorker.Status.Equals("reject"))
      {
        return BadRequest("This order has already been resolved");
      }
      else if (orderOfWorker.Status.Equals("pending") && worker.WorkingState.Equals("working"))
      {
        orderOfWorker.Status = "inprogress";
        if (await _workerRepository.SaveAllAsync()) return Ok("The worker approve booking successfully");

        return BadRequest("Problem approve the booking");
      }
      else if (orderOfWorker.Status.Equals("inprogress") && worker.WorkingState.Equals("working"))
      {
        orderOfWorker.Status = "finished";
        worker.WorkingState = "free";
        if (await _workerRepository.SaveAllAsync())
        {
          // Send Mail for user when finish the order
          string path = @"MailContent\Review.html";
          // set up to send mail 
          string bodyContent = ReadFileHelper.ReadFile(path);
          bodyContent = bodyContent.Replace("GuestName", orderOfWorker.GuestName);
          var mailContent = new MailContent()
          {
            Subject = "Tell Us How We Did - Leave a Review",
            Body = bodyContent,
            To = orderOfWorker.GuestEmail
          };
          // send mail
          await _sendMailService.SendMailAsync(mailContent);
          return Ok("The worker finishes booking successfully");
        }
        return BadRequest("Problem finishing the booking");
      }
      else
      {
        return BadRequest("You need to check your Working State");
      }
    }

    [HttpDelete("cancel-order/{orderId}")]
    [Authorize(Roles = "worker")]
    public async Task<ActionResult> CancelOrderOfWorker(int orderId)
    {
      var userId = User.FindFirst("userId")?.Value;
      var worker = await _workerRepository.GetWorkerEntityByIdAsync(int.Parse(userId));

      if (worker is null) return NotFound();

      var orderOfWorker = await _orderHistoryRepository.GetOrderHistoryAsync(orderId);
      if (orderOfWorker is null) return NotFound();

      if (orderOfWorker.WorkerId != worker.Id) return BadRequest("You are not a worker to serve this order");

      if (orderOfWorker.Status.Equals("finished") || orderOfWorker.Status.Equals("reject"))
      {
        return BadRequest("This has already been resolved");
      }
      else if (orderOfWorker.Status.Equals("inprogress") && worker.WorkingState.Equals("working"))
      {
        return BadRequest("This order is progressing");
      }
      else if (orderOfWorker.Status.Equals("pending") && worker.WorkingState.Equals("working"))
      {
        orderOfWorker.Status = "reject";
        worker.WorkingState = "off";
        if (await _workerRepository.SaveAllAsync())
        {
          // Send Mail for user when the worker reject the order
          string path = @"MailContent\RejectMail.html";
          // set up to send mail 
          string bodyContent = ReadFileHelper.ReadFile(path);
          bodyContent = bodyContent.Replace("GuestUser", orderOfWorker.GuestName);
          var mailContent = new MailContent()
          {
            Subject = "Order Rejected: Worker Unavailable",
            Body = bodyContent,
            To = orderOfWorker.GuestEmail
          };
          // send mail
          await _sendMailService.SendMailAsync(mailContent);
          return Ok("The worker reject booking successfully");
        }
        return BadRequest("Problem finishing the booking");
      }
      else
      {
        return BadRequest("You need to check your Working State");
      }
    }


    [HttpPut("working-state")]
    [Authorize(Roles = "worker")]
    public async Task<ActionResult> SwitchWorkingStateOfWorker()
    {
      var userId = User.FindFirst("userId")?.Value;
      var worker = await _workerRepository.GetWorkerEntityByIdAsync(int.Parse(userId));

      if (worker is null) return NotFound();

      if (worker.WorkingState.Equals("working")) return BadRequest("You are working so that you can not do it.");

      if (worker.WorkingState.Equals("free"))
      {
        worker.WorkingState = "off";
      }
      else
      {
        worker.WorkingState = "free";
      }

      if (await _workerRepository.SaveAllAsync()) return Ok();
      return BadRequest("Problem switching state");
    }

    [HttpPost("update")]
    [Authorize(Roles = "worker")]
    public async Task<ActionResult> TrackingWorkerInfo(WorkerRegisterDto workerRegisterDto)
    {
      var workerId = Int32.Parse(User.FindFirst("userId")?.Value);
      //Get worker entity

      var worker = await _workerRepository.GetWorkerEntityByIdAsync(workerId, includeTrackingWorker: true);
      if (worker is null) return NotFound();
      // add new info into database
      var list = workerRegisterDto.ChoresList.Select(chore =>
      {
        return new TrackingWorker
        {
          WorkerId = workerId,
          ChoreId = chore,
          Fee = workerRegisterDto.Fee
        };
      }).ToList();
      if (worker.TrackingWorker != null)
      {
        worker.TrackingWorker = list;
      }
      else
      {
        worker.TrackingWorker.AddRange(list);
      }
      if (await _workerRepository.SaveAllAsync()) return Ok();
      return BadRequest("Problem in request update worker info");
    }

    [HttpGet("admin/track")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<Object>>> RequestWorkerInfoAdminView()
    {
      var workers = await _workerRepository.GetWorkersEntityInTrackingWorkerAsync(includeTrackingWorker: true, includeUser: true, includeWorkersChores: true);
      if (workers.Count() == 0)
      {
        return BadRequest("Not Tracking Workers were found");
      }
      List<object> resultList = new();
      foreach (var worker in workers)
      {
        var result = new
        {
          WorkerId = worker.Id,
          WorkerName = worker.User.Name,
          OldFee = worker.Fee,
          NewFee = worker.TrackingWorker.FirstOrDefault(x => x.WorkerId == worker.Id).Fee,
          OldChores = worker.Workers_Chores.Where(x => x.WorkerId == worker.Id).Select(x => x.Chore).ToList(),
          NewChores = worker.TrackingWorker.Where(x => x.WorkerId == worker.Id).Select(x => x.Chore).ToList(),
        };
        resultList.Add(result);
      }
      return Ok(resultList);
    }
  }
}
