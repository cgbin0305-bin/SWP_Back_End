using API.DTOs;

namespace API.Interfaces;

public interface IChoresRepository
{
    Task<IEnumerable<HouseHoldChoresDto>> GetHouseHoldChoresDtos();
}