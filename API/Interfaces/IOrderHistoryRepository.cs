
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IOrderHistoryRepository
    {
        Task<IEnumerable<OrderHistoryDto>> GetAllOrderHistoriesAsync();

        Task<IEnumerable<OrderHistoryDto>> SearchOrderHistoriesAsync(string keyword);
    }
}