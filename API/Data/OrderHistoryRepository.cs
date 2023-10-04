
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;


namespace API.Data
{
    public class OrderHistoryRepository : IOrderHistoryRepository
    {
        private readonly WebContext _webContext;
        private readonly IMapper _mapper;

        public OrderHistoryRepository(WebContext webContext, IMapper mapper)
        {
            _webContext = webContext;
            _mapper = mapper;
        }
        public async Task<IEnumerable<OrderHistoryDto>> GetAllOrderHistoriesAsync()
        {
            return await _webContext.OrderHistories
                    .ProjectTo<OrderHistoryDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
        }
    }
}