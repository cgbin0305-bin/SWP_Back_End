
using API.DTOs;
using API.Helper;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


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
            var orderHistories =(List<OrderHistoryDto>) await _orderHistoryRepository.SearchOrderHistoriesAsync(keyword.ToLower());

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
    }

}
