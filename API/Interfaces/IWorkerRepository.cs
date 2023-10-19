using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IWorkerRepository
{
    Task<bool> SaveAllAsync();

    Task<WorkerDto> GetWorkerByIdAsync(int id);

    Task<IEnumerable<WorkerDto>> GetAllWorkersAsync();

    Task<IEnumerable<WorkerDto>> SearchWorkersAsync(string keyword);

    Task<IEnumerable<WorkerDto>> SearchWorkersByAdminAsync(string keyword);

    Task<IEnumerable<WorkerDto>> GetAllWorkersForAdminAsync();

    Task<Worker> GetWorkerEntityByIdAsync(int id);

    Task<bool> UpdateWorkerStatusAsync(WorkerStatusDto dto);

}