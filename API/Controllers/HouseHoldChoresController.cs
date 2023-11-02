
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class HouseHoldChoresController : BaseApiController
  {
    private readonly IChoresRepository _choresRepository;

    public HouseHoldChoresController(IChoresRepository choresRepository)
    {
      _choresRepository = choresRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<HouseHoldChoresDto>> GetALlChores()
    {
      return await _choresRepository.GetHouseHoldChoresDtos();
    }
  }
}