//using Azure;
//using Azure.Core;
//using JSE.Data;
//using Microsoft.AspNetCore.Mvc;

//namespace JSE.Controllers {
//    [ApiController]
//    [Route("[controller]")]
//    public class DeliveryController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//         public DeliveryController(AppDbContext context)
//        {
//            _context = context;
//        }


//        [HttpGet("")]

//        public async Task<ActionResult<IEnumerable<GetDeliveryResult>>> Get()
//        {
//            var delivery = await _context.Delivery
//            .OrderBy(× => x.tracking_number)
//            .Select(x => new GetDeliveryResult(){
//                tracking_number = x.


//            }).ToListAsync();
//            var response = new ApiResponse<IEnumerable<GetCategoryResult>>
//                3;
//            StatusCode = StatusCodes.Status2000K,
//        RequestMethod = HttpContext.Request.Method,
//        Payload = category
//        return Ok(response);
//        / GET api / values / 5
//        [HttpGet("{id?")] //ambil sesuai parameter
//                public async Task<ActionResult<GetCategorvResult>> Get(int id)
//        var category = await _context2.Category

//        }


//    };
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
            return Ok("success test");
        }
    }
}





