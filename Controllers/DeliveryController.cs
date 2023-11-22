using JSE.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JSE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeliveryController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DeliveryController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("")]
        public async Task<IActionResult> GetDeliveries()
        {
            var query = await _context.Delivery.ToListAsync();
            return new ObjectResult(query) {
                StatusCode = 200,
            };
        }
    }
}





