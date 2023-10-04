
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

            List<OrderHistoryDto> list = new List<OrderHistoryDto>();

            foreach (var item in result.Skip(currentPage * (int)pageSize).Take((int)pageSize).ToList())
            {
                OrderHistoryDto dto = new OrderHistoryDto()
                {
                    Rate = item.Rate,
                    Date = item.Date,
                    GuestAddress = item.GuestAddress,
                    GuestEmail = item.GuestEmail,
                    GuestName = item.GuestName,
                    GuestPhone = item.GuestPhone,
                    Id = item.Id,
                    WorkerName = item.WorkerName,
                    WorkerId = item.WorkerId
                };
                list.Add(dto);
            }
            OrderHistoryByPage orderHistoryByPage = new OrderHistoryByPage()
            {
                OrderHistories = list,
                CurrentPage = currentPage,
                PageSize = (int)pageSize,
                TotalElements = result.Count
            };
            return Ok(orderHistoryByPage);
        }
    }
}