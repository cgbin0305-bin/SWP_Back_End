using API.DTOs;

namespace API.Interfaces;

public interface IWorkerRepository
{
    Task<bool> SaveAllAsync();

    Task<WorkerDto> GetWorkerByIdAsync(int id);

    Task<IEnumerable<WorkerDto>> GetAllWorkers();

    Task<IEnumerable<WorkerDto>> SearchWorkersAsync(string keyword);
    
}