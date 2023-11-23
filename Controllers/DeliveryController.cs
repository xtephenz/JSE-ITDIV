using JSE.Data;
using JSE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JSE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeliveryController : Controller
    {
        private readonly AppDbContext _context;
        public DeliveryController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("/daftar-pesanan"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeliveries([FromBody] Guid admin_id)
        {
            try
            {
                var deliveries = await _context.Admin.Where(c => c.admin_id == admin_id).ToListAsync();
                return new ObjectResult(deliveries)
                {
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        
    }
}





