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
                var adminObject = await _context.Admin.FindAsync(admin_id);
                var deliveries = await _context.Delivery.Where(c => c.pool_sender_city == adminObject.pool_city && c.pool_receiver_city == adminObject.pool_city).ToListAsync();
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
        [HttpGet("")]
        public async Task<IActionResult> GetAllDeliveries()
        {
            try
            {
                var deliveries = await _context.Delivery.ToListAsync();
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





