
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IOrderHistoryRepository
    {
        Task<IEnumerable<OrderHistoryDto>> GetAllOrderHistoriesAsync();

        Task<IEnumerable<OrderHistoryDto>> SearchOrderHistoriesAsync(string keyword);

        Task<IEnumerable<OrderHistoryOfUserDto>> GetOrderHistoriesByEmailAsync(string email, string phone);

        Task<OrderHistory> GetOrderHistoryAsync(int OrderId);

        (int, int) CountOrderAndRateOfWorker(int workerId);
    }
}