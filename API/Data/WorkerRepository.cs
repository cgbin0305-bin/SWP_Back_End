using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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

    public async Task<IEnumerable<WorkerDto>> GetAllWorkersAsync(string address)
    {

        var query = _context.Workers.Include(x => x.User).Where(x => x.Status && x.WorkingState == "free");

        if (!string.IsNullOrEmpty(address))
        {
            query = query.OrderBy(worker => worker.User.Address == address ? 0 : 1) // User's address first
            .ThenBy(worker => worker.User.Address); // Alphabetical order for other addresses
        }

        return await query.ProjectTo<WorkerDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkerDto>> GetAllWorkersForAdminAsync()
    {
        return await _context.Workers
            .OrderBy(x => x.Id)
            .ProjectTo<WorkerDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<WorkerDto> GetWorkerByIdAsync(int id)
    {
        return await _context.Workers
                    .Where(x => x.Id == id && x.Status && x.WorkingState == "free")
                    .ProjectTo<WorkerDto>(_mapper.ConfigurationProvider)
                    .AsSplitQuery()
                    .SingleOrDefaultAsync();
    }

    public async Task<Worker> GetWorkerEntityByIdAsync(int id, bool includeOrderHistories = false, bool includeUser = false, bool includeWorkersChores = false, bool includeTrackingWorker = false)
    {
        var query = _context.Workers.Where(x => x.Id == id);

        if (includeOrderHistories)
        {
            query = query.Include(x => x.OrderHistories)
                .ThenInclude(x => x.Review);
        }

        if (includeUser)
        {
            query = query.Include(x => x.User);
        }

        if (includeWorkersChores)
        {
            query = query.Include(x => x.Workers_Chores)
                .ThenInclude(x => x.Chore);
        }
        if (includeTrackingWorker)
        {
            query = query.Include(x => x.TrackingWorker)
                .ThenInclude(x => x.Chore);
        }
        return await query.FirstOrDefaultAsync();
    }


    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    private IQueryable<Worker> SearchQueryWorker(string keyword)
    {
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
        return await SearchQueryWorker(keyword).Where(x => x.Status && x.WorkingState.ToLower() == "free")
        .ProjectTo<WorkerDto>(_mapper.ConfigurationProvider)
        .ToListAsync();
    }

    public async Task<IEnumerable<WorkerDto>> SearchWorkersByAdminAsync(string keyword)
    {
        return await SearchQueryWorker(keyword)
        .OrderBy(x => x.Id)
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

    public async Task<IEnumerable<Worker>> GetWorkersEntityInTrackingWorkerAsync(bool includeOrderHistories = false, bool includeUser = false, bool includeWorkersChores = false, bool includeTrackingWorker = false)
    {
        var workersId = _context.TrackingWorker.GroupBy(x => x.WorkerId);

        List<Worker> list = new List<Worker>();

        foreach (var workerId in workersId)
        {
            var query = _context.Workers.Where(x => x.Id == workerId.Key);
            if (includeOrderHistories)
            {
                query = query.Include(x => x.OrderHistories)
                   .ThenInclude(x => x.Review);
            }

            if (includeUser)
            {
                query = query.Include(x => x.User);
            }

            if (includeWorkersChores)
            {
                query = query.Include(x => x.Workers_Chores)
                    .ThenInclude(x => x.Chore);
            }
            if (includeTrackingWorker)
            {
                query = query.Include(x => x.TrackingWorker)
                    .ThenInclude(x => x.Chore);
            }
            list.Add(await query.SingleOrDefaultAsync());
        }

        return list;
    }
}