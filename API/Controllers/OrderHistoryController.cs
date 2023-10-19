
using API.DTOs;
using API.Helper;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using API.Entities;


namespace API.Controllers
{
    public class OrderHistoryController : BaseApiController
    {
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWorkerRepository _workerRepository;
        private readonly IMapper _mapper;

        public OrderHistoryController(IOrderHistoryRepository orderHistoryRepository, IUserRepository userRepository, IWorkerRepository workerRepository, IMapper mapper)
        {
            _orderHistoryRepository = orderHistoryRepository;
            _userRepository = userRepository;
            _workerRepository = workerRepository;
            _mapper = mapper;
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

            var orderHistory = _mapper.Map<OrderHistory>(dto);

            var worker = await _workerRepository.GetWorkerEntityByIdAsync(dto.WorkerId);

            if(worker == null) return BadRequest("Worker does not exist");

            worker.OrderHistories.Add(orderHistory);

            if (await _workerRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetOrderHistoryByPage),
                new { }, _mapper.Map<OrderHistoryDto>(orderHistory));
            }

            return BadRequest("Problem hiring the worker");

        }
    }

}
