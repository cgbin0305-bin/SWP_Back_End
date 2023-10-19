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

    private IQueryable<Worker> SearchQueryWorker(string keyword) {
         var query = _context.Workers
            .Include(x => x.OrderHistories)
            .Include(x => x.User)
            .Include(x => x.Workers_Chores)
                .ThenInclude(x => x.Chore)
            .AsQueryable();

        query = query.Where(x => x.User.Name.ToLower().Contains(keyword)
            || x.User.Address.ToLower().Contains(keyword)
            || x.Workers_Chores.Any(chore => chore.Chore.Name.ToLower().Contains(keyword) 
            || chore.Chore.Description.ToLower().Contains(keyword)));

        return query;
    } 

    public async Task<IEnumerable<WorkerDto>> SearchWorkersAsync(string keyword)
    {
        return await SearchQueryWorker(keyword).Where(x => x.Status)
        .ProjectTo<WorkerDto>(_mapper.ConfigurationProvider)
        .ToListAsync();
    }

    public async Task<IEnumerable<WorkerDto>> SearchWorkersByAdminAsync(string keyword)
    {
        return await SearchQueryWorker(keyword)
        .ProjectTo<WorkerDto>(_mapper.ConfigurationProvider)
        .ToListAsync();
    }

    public async Task<bool> UpdateWorkerStatusAsync(WorkerStatusDto dto)
    {
        var worker = await _context.Workers.Where(w => w.Id == dto.WorkerId).SingleOrDefaultAsync();
        if (worker != null)
        {
            if (!worker.Version.Equals(new Guid(dto.Version)))
            {
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