
using API.DTOs;

namespace API.Interfaces
{
    public interface IOrderHistoryRepository
    {
        Task<IEnumerable<OrderHistoryDto>> GetAllOrderHistoriesAsync();
    }
}