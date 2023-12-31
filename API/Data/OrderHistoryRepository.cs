
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

        public async Task<OrderHistory> GetOrderHistoryAsync(int OrderId)
        {
            return await _context.OrderHistories
                .Include(x => x.Worker)
                    .ThenInclude(x => x.User)
                .Include(x => x.Review)
                .Where(x => x.Id == OrderId)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderHistoryDto>> GetAllOrderHistoriesAsync()
        {
            return await _context.OrderHistories
                    .OrderByDescending(x => x.Date)
                    .ProjectTo<OrderHistoryDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
        }

        public async Task<IEnumerable<OrderHistoryDto>> SearchOrderHistoriesAsync(string keyword)
        {
            var query = _context.OrderHistories
                .Include(x => x.Worker)
                    .ThenInclude(x => x.User)
                .Include(x => x.Review)
                .AsQueryable();

            query = query.Where(x => x.GuestName.ToLower().Contains(keyword)
            || x.GuestEmail.ToLower().Contains(keyword)
            || x.GuestAddress.ToLower().Contains(keyword)
            || x.GuestPhone.Contains(keyword)
            || x.Worker.User.Name.ToLower().Contains(keyword)
            || x.Status.Contains(keyword));

            return await query
            .OrderByDescending(x => x.Date)
            .ProjectTo<OrderHistoryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        }

        public async Task<IEnumerable<OrderHistoryOfUserDto>> GetOrderHistoriesByEmailAsync(string email, string phone)
        {
            return await _context.OrderHistories
            .Where(x => x.GuestEmail == email && x.GuestPhone == phone)
            .OrderByDescending(x => x.Date)
            .ProjectTo<OrderHistoryOfUserDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        }

        public (int, int) CountOrderAndRateOfWorker(int workerId) {
            var listOrder = _context.OrderHistories.Where(x => x.WorkerId == workerId).Select(x => x.Id).ToList();

            if(listOrder.Count() == 0) return (0,0);

            var rate = _context.Reviews
                .Where(x => listOrder.Contains(x.Id))
                .Select(x => x.Rate).ToList();
            return (listOrder.Count(), (int)rate.Average());
        }
    }
}