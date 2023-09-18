
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class HouseHoldChoresController : BaseApiController
    {
        private readonly WebContext _context;
        public HouseHoldChoresController(WebContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HouseHoldChoresDto>>> GetRoles()
        {
            var result = await _context.HouseHoldChores.ToListAsync();
            List<HouseHoldChoresDto> list = new List<HouseHoldChoresDto>();
            foreach (var item in result)
            {
                HouseHoldChoresDto dto = new HouseHoldChoresDto()
                {
                    Id = item.Id,
                    Name = item.ChoresName,
                    Description = item.Description
                };
                list.Add(dto);
            }

            return list;
        }
    }
}