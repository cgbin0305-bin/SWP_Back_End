using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class WorkerRepository : IWorkerRepository
{
    private readonly WebContext _context;

    private readonly IMapper _mapper;

    public WorkerRepository(WebContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WorkerDto>> GetAllWorkers()
    {
        return await _context.Workers
            .Where(x => x.Status)
            .ProjectTo<WorkerDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<WorkerDto> GetWorkerByIdAsync(int id)
    {
        return await _context.Workers
            .Where(x => x.Id == id && x.Status)
            .ProjectTo<WorkerDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<WorkerDto>> SearchWorkersAsync(string keyword)
    {
        return null;
    }
}