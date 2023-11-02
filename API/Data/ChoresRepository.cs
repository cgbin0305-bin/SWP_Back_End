using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace API.Data;

public class ChoresRepository : IChoresRepository
{
    private readonly WebContext _context;
    private readonly IMapper _mapper;

    public ChoresRepository(WebContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<HouseHoldChoresDto>> GetHouseHoldChoresDtos()
    {
        return await _context.HouseHoldChores
                .ProjectTo<HouseHoldChoresDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
      
    }
}