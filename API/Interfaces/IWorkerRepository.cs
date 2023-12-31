using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IWorkerRepository
{
    Task<bool> SaveAllAsync();

    Task<WorkerDto> GetWorkerByIdAsync(int id);

    Task<IEnumerable<WorkerDto>> GetAllWorkersAsync(string address);

    Task<IEnumerable<WorkerDto>> SearchWorkersAsync(string keyword);

    Task<IEnumerable<WorkerDto>> SearchWorkersByAdminAsync(string keyword);

    Task<IEnumerable<WorkerDto>> GetAllWorkersForAdminAsync();
    Task<IEnumerable<Worker>> GetWorkersEntityInTrackingWorkerAsync(bool includeOrderHistories = false, bool includeUser = false, bool includeWorkersChores = false, bool includeTrackingWorker = false);

    Task<Worker> GetWorkerEntityByIdAsync(int id, bool includeOrderHistories = false, bool includeUser = false, bool includeWorkersChores = false, bool includeTrackingWorker = false);

    Task<bool> UpdateWorkerStatusAsync(WorkerStatusDto dto);
}