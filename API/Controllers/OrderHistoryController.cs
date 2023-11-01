
using API.DTOs;
using API.Helper;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using API.Entities;
using Microsoft.VisualBasic;
using API.Services;
using API.Errors;


namespace API.Controllers
{
    public class OrderHistoryController : BaseApiController
    {
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWorkerRepository _workerRepository;
        private readonly IMapper _mapper;
        private readonly ISendMailService _sendMailService;

        public OrderHistoryController(IOrderHistoryRepository orderHistoryRepository, IUserRepository userRepository, IWorkerRepository workerRepository, IMapper mapper, ISendMailService sendMailService)
        {
            _orderHistoryRepository = orderHistoryRepository;
            _userRepository = userRepository;
            _workerRepository = workerRepository;
            _mapper = mapper;
            _sendMailService = sendMailService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<EntityByPage<OrderHistoryDto>>> GetOrderHistoryByPage([FromQuery(Name = "page")] string pageString)
        {
            var result = await _orderHistoryRepository.GetAllOrderHistoriesAsync();
            if (result is null)
            {
                return BadRequest("No OrderHistory!");
            }

            var resultPage = MapEntityHelper.MapEntityPaginationAsync(pageString, result, 25f);
            return Ok(resultPage);
        }


        [HttpGet("search")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<EntityByPage<OrderHistoryDto>>> SearchOrderHistories([FromQuery(Name = "keyword")] string keyword, [FromQuery(Name = "page")] string pageString)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return NotFound();
            }

            var orderHistories = await _orderHistoryRepository.SearchOrderHistoriesAsync(keyword.ToLower());

            var resultPage = MapEntityHelper.MapEntityPaginationAsync(pageString, orderHistories, 25f);

            return Ok(resultPage);
        }

        [HttpPost("hire")]
        public async Task<ActionResult<OrderHistoryDto>> HireWorker(HireWorkerInfoDto dto)
        {
            var worker = await _workerRepository.GetWorkerEntityByIdAsync(dto.WorkerId, true, true);
            var workerInfo = await _userRepository.GetUserByIdAsync(worker.Id);
            if (worker == null) return BadRequest("Worker does not exist");
            if (worker.WorkingState == "working") throw new WorkerIsWorkingException();

            var userId = User.FindFirst("userId")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userRepository.GetUserByIdAsync(int.Parse(userId));
                dto.GuestName = user.Name;
                dto.GuestPhone = user.Phone;
                dto.GuestEmail = user.Email;
                dto.GuestAddress = user.Address;
            }

            if (string.IsNullOrWhiteSpace(dto.GuestName) || string.IsNullOrWhiteSpace(dto.GuestPhone) || string.IsNullOrWhiteSpace(dto.GuestEmail) || string.IsNullOrWhiteSpace(dto.GuestAddress))
            {
                return BadRequest("Some fields of Guest are empty");
            }
            string path = @"MailContent\HireWorker.html";
            // set up to send mail 
            string bodyContent = ReadFileHelper.ReadFile(path);
            bodyContent = bodyContent.Replace("GuestName", dto.GuestName);
            bodyContent = bodyContent.Replace("GuestPhone", dto.GuestPhone);
            bodyContent = bodyContent.Replace("GuestAddress", dto.GuestAddress);
            bodyContent = bodyContent.Replace("WorkerName", workerInfo.Name);
            var mailContent = new MailContent()
            {
                Subject = "Hiring Request: Guest Information",
                Body = bodyContent,
                To = workerInfo.Email
            };
            // send mail
            await _sendMailService.SendMailAsync(mailContent);

            var orderHistory = _mapper.Map<OrderHistory>(dto);


            worker.WorkingState = "working";
            worker.OrderHistories.Add(orderHistory);

            if (await _workerRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetOrderHistoryByPage),
                new { }, _mapper.Map<OrderHistoryDto>(orderHistory));
            }

            return BadRequest("Problem hiring the worker");
        }

        [HttpPost("review")]
        public async Task<ActionResult> PostReviewForWorker(ReviewOfUserDto reviewOfUserDto)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userRepository.GetUserEntityByIdAsync(int.Parse(userId));
                reviewOfUserDto.Email = user.Email;
            }

            var orderHistory = await _orderHistoryRepository.GetOrderHistoryAsync(reviewOfUserDto.OrderId);

            if (orderHistory is null) return NotFound();

            if (orderHistory.GuestEmail != reviewOfUserDto.Email) return BadRequest("You can not review this because you are not the one booking the service.");

            var newReview = new Review
            {
                Id = orderHistory.Id,
                Content = reviewOfUserDto.ReviewContent,
                Rate = reviewOfUserDto.Rate > 5 || reviewOfUserDto.Rate < 0 ? 0 : reviewOfUserDto.Rate,
                Date = DateTime.UtcNow
            };

            orderHistory.Review = newReview;

            if (await _userRepository.SaveChangeAsync()) return Ok();

            return BadRequest("Problem sending a review");

        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderHistoryOfUserDto>> GetOrderByOrderIdOfUser(int orderId)
        {
            //get order
            var order = await _orderHistoryRepository.GetOrderHistoryAsync(orderId);
            if (order is null)
            {
                return NotFound();
            }
            return _mapper.Map<OrderHistoryOfUserDto>(order);
        }
    }
}
