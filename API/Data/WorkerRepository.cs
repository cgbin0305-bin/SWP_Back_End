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

    public async Task<IEnumerable<WorkerDto>> GetAllWorkersAsync()
    {
        return await _context.Workers
            .Where(x => x.Status)
            .ProjectTo<WorkerDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkerDto>> GetAllWorkersForAdminAsync()
    {
        return await _context.Workers
            .ProjectTo<WorkerDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<WorkerDto> GetWorkerByIdAsync(int id)
    {
        return await _context.Workers
            .Where(x => x.Id == id && x.Status)
            .ProjectTo<WorkerDto>(_mapper.ConfigurationProvider)
            .AsSplitQuery()
            .SingleOrDefaultAsync();
    }

    public async Task<Worker> GetWorkerEntityByIdAsync(int id)
    {
        return await _context.Workers
            .Where(x => x.Id == id)
            .Include(x => x.OrderHistories)
            .Include(x => x.User)
            .Include(x => x.Workers_Chores)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<WorkerDto>> SearchWorkersAsync(string keyword)
    {
        // keyword is name, address or chores
        var workers = await _context.Workers
            .Where(x => x.Status)
            .ProjectTo<WorkerDto>(_mapper.ConfigurationProvider)
            .AsQueryable()
            .ToListAsync();

        var result = workers.Where(x => x.Name.ToLower().Contains(keyword)
            || x.Address.ToLower().Contains(keyword)
            || x.Chores.Any(chore => chore.Name.ToLower().Contains(keyword)
            || chore.Description.ToLower().Contains(keyword)));

        return result;
    }
    public async Task<bool> UpdateWorkerStatusAsync(WorkerStatusDto dto)
    {
        var worker = await _context.Workers.Where(w => w.Id == dto.WorkerId).SingleOrDefaultAsync();
        if (worker != null)
        {
            if(!worker.Version.Equals(new Guid(dto.Version))) {
                throw new InvalidOperationException("Concurrency conflict detected. Please reload the data.");
            }
            worker.Status = dto.Status;
            worker.Version = Guid.NewGuid();
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

}