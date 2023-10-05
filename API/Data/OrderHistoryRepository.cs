
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
        private readonly WebContext _context;
        private readonly IMapper _mapper;

        public OrderHistoryRepository(WebContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<OrderHistoryDto>> GetAllOrderHistoriesAsync()
        {
            return await _context.OrderHistories
                    .ProjectTo<OrderHistoryDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
        }

        public async Task<IEnumerable<OrderHistoryDto>> SearchOrderHistoriesAsync(string keyword)
        {
            return await _context.OrderHistories
            .Where(x => x.GuestName.ToLower().Contains(keyword)
            || x.GuestEmail.ToLower().Contains(keyword)
            || x.GuestAddress.ToLower().Contains(keyword)
            || x.GuestPhone.Contains(keyword))
            .ProjectTo<OrderHistoryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        }
    }
}