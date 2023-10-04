
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    public class OrderHistoryController : BaseApiController
    {
        private readonly IOrderHistoryRepository _orderHistoryRepository;

        public OrderHistoryController(IOrderHistoryRepository orderHistoryRepository)
        {
            _orderHistoryRepository = orderHistoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderHistoryDto>>> GetAllOrderHistories()
        {
            var result = (List<OrderHistoryDto>)await _orderHistoryRepository.GetAllOrderHistoriesAsync();
            if (result is null)
            {
                return BadRequest("No OrderHistory!");
            }
            return result;
        }
    }
}