
using API.DTOs;
using API.Helper;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace API.Controllers
{
    public class OrderHistoryController : BaseApiController
    {
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly IUserRepository _userRepository;

        public OrderHistoryController(IOrderHistoryRepository orderHistoryRepository, IUserRepository userRepository)
        {
            _orderHistoryRepository = orderHistoryRepository;
            _userRepository = userRepository;
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
        public async Task<ActionResult<string>> HireWorker(HireWorkerInfoDto dto)
        {
            int userId;
            UserDto user;
            try
            {
                //get userId from token
                userId = Int32.Parse(User.FindFirst("userId")?.Value);
                // get user by id
                user = await _userRepository.GetUserByIdAsync(userId);
                dto.GuestName = user.Name;
                dto.GuestPhone = user.Phone;
                dto.GuestEmail = user.Email;
                dto.GuestAddress = user.Address;
            }
            catch (System.Exception)
            {
                // add order history 
                if (await _orderHistoryRepository.AddOrderHistoryAsync(dto))
                {
                    return Ok("Hire Worker without log in Successfully.");

                }
                return BadRequest();

            }

            // add order history 
            if (await _orderHistoryRepository.AddOrderHistoryAsync(dto))
            {
                return Ok("Hire Worker with login Successfully.");
            }
            return BadRequest();
        }
    }

}
