
using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Helper
{
    public class SearchWorkerHelper
    {
        public static WorkersByPage MapWorkerPaginationAsync(string pageString, IEnumerable<WorkerDto> workers)
        {
            int currentPage;
            float pageSize = 12f;

            try
            {
                currentPage = PageHelper.CurrentPage(pageString, workers.Count(), pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("Search Worker Helper: " + ex.Message);
            }

            var listWorkerPage = new List<WorkerPage>();
            foreach (var worker in workers.Skip(currentPage * (int)pageSize).Take((int)pageSize).ToList())
            {
                var workerPage = new WorkerPage
                {
                    Id = worker.Id,
                    Name = worker.Name,
                    Fee = worker.Fee,
                    Status = worker.Status,
                    Address = worker.Address,
                    AverageRate = worker.AverageRate,
                    Chores = worker.Chores
                };
                listWorkerPage.Add(workerPage);
            }

            var resultPage = new WorkersByPage()
            {
                Workers = listWorkerPage,
                CurrentPage = currentPage,
                TotalElements = workers.Count(),
                PageSize = (int)pageSize
            };
            return resultPage;
        }
    }
}