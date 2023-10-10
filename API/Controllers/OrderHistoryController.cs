
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
        public async Task<ActionResult<OrderHistoryByPage>> GetOrderHistoryByPage([FromQuery(Name = "page")] string pageString)
        {
            int currentPage;
            float pageSize = 25f;
            var result = (List<OrderHistoryDto>)await _orderHistoryRepository.GetAllOrderHistoriesAsync();
            if (result is null)
            {
                return BadRequest("No OrderHistory!");
            }
            try
            {
                currentPage = PageHelper.CurrentPage(pageString, result.Count, pageSize);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            OrderHistoryByPage orderHistoryByPage = new OrderHistoryByPage
            {
                OrderHistories = result.Skip(currentPage * (int)pageSize).Take((int)pageSize).ToList(),
                CurrentPage = currentPage,
                PageSize = (int)pageSize,
                TotalElements = result.Count
            };
            return Ok(orderHistoryByPage);
        }


        [HttpGet("search")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<OrderHistoryByPage>> SearchOrderHistories([FromQuery(Name = "keyword")] string keyword, [FromQuery(Name = "page")] string pageString)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return NoContent();
            }

            int currentPage;
            float pageSize = 25f;
            var orderHistories = (List<OrderHistoryDto>)await _orderHistoryRepository.SearchOrderHistoriesAsync(keyword.ToLower());

            try
            {
                currentPage = PageHelper.CurrentPage(pageString, orderHistories.Count(), pageSize);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var resultPage = new OrderHistoryByPage
            {
                OrderHistories = orderHistories.Skip(currentPage * (int)pageSize).Take((int)pageSize).ToList(),
                CurrentPage = currentPage,
                TotalElements = orderHistories.Count(),
                PageSize = (int)pageSize
            };

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

            if (dto.GuestName == null || dto.GuestEmail == null || dto.GuestEmail == null || dto.GuestAddress == null)
            {
                return BadRequest("Some fields of Guest are empty");
            }

            var orderHistory = _mapper.Map<OrderHistory>(dto);

            var worker = await _workerRepository.GetWorkerEntityByIdAsync(dto.WorkerId);

            worker.OrderHistories.Add(orderHistory);

            if(await _workerRepository.SaveAllAsync()) {
                return CreatedAtAction(nameof(GetOrderHistoryByPage),
                new {}, _mapper.Map<OrderHistoryDto>(orderHistory));
            }

            return BadRequest("Problem hiring the worker");
        
        }
    }

}
