using AutoMapper;
using AutoMapper.QueryableExtensions;
using JSE.Data;
using JSE.Models;
using JSE.Models.Requests;
using JSE.Models.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper.QueryableExtensions;
using System.Security.Cryptography;
using System.Net;

namespace JSE.Controllers
{
    [Route("courier")]
    [ApiController]
    public class CourierController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;


        public CourierController(IConfiguration configuration, IMapper mapper, AppDbContext context)

        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCouriers()
        {
            var couriers = await _context.Courier.ProjectTo<GetCourierResult>(_mapper.ConfigurationProvider).ToListAsync();
            return Ok(couriers);
        }
        [HttpGet("profile"), Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            var courierData = User?.Identity?.Name;
            return Ok(new { courierData });
        }
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST
        ///     {
        ///         "courier_username": "meerkat",
        ///         "courier_password": "Bl@ck$",
        ///         "courier_confirm_password": "Bl@ck$",
        ///         "courier_phone": "111111"
        ///     }
        /// </remarks>
        /// <param name="register"></param>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] CourierRegisterRequest register)
        {
            if (register.courier_password != register.courier_confirm_password) return new ObjectResult(new { message = "Password mismatch!" })
            { StatusCode = 400 };

            var userExists = await _context.Courier.Where(c => c.courier_username == register.courier_username).ToListAsync();
            if (userExists.Count == 0)
            {
                register.courier_password = BCrypt.Net.BCrypt.EnhancedHashPassword(register.courier_password, 13);
                var newCourier = new Courier()
                {
                    courier_username = register.courier_username,
                    courier_password = register.courier_password,
                    courier_phone = register.courier_phone,
                };
                _context.Courier.Add(newCourier);
                await _context.SaveChangesAsync();
                return Ok(register);
            }
            else
            {
                return new ObjectResult(new { message = "Username already exists!" })
                {
                    StatusCode = 400
                };
            }

        }
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST
        ///     {
        ///         "courier_username": "meerkat",
        ///         "courier_password": "Bl@ck$"
        ///     }
        /// </remarks>
        /// <param name="login"></param>
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] CourierLoginRequest login)
        {
            var CheckUser = await _context.Courier.Where(c => c.courier_username == login.courier_username).FirstOrDefaultAsync();
            if (CheckUser != null)
            {
                var PasswordCheck = BCrypt.Net.BCrypt.EnhancedVerify(login.courier_password, CheckUser.courier_password);
                // var PasswordCheck = login.admin_password == CheckUser[0].admin_password;
                if (PasswordCheck)
                {
                    var token = CreateToken(login.courier_username, CheckUser.courier_id);
                    var responseData = new { token = token };
                    return new ObjectResult(responseData)
                    {
                        StatusCode = 200,
                    };
                }
                else
                {
                    var responseData = new { message = "Login Unsuccessful, email or password invalid!" };
                    return new ObjectResult(responseData)
                    {
                        StatusCode = 401,
                    };
                }
            }
            else
            {
                var responseData = new { message = "Login Unsuccessful, email or password invalid!" };
                return new ObjectResult(responseData)
                {
                    StatusCode = 401,
                };
            }
        }
        private string CreateToken(String username, Guid UserId)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, username.ToString()),
                new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
                new Claim(ClaimTypes.Role, "Courier"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("JwtSettings:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,

                    expires: DateTime.Now.AddHours(6),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        //[HttpGet(""), Authorize(Roles = "Courier")]
        [HttpGet("history"), Authorize]
        public async Task<IActionResult> GetDeliveryListCourier()
        {
            try
            {
                var courier_id = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                var DeliveryList = await _context.Delivery.Where(c => c.courier_id == courier_id).ToListAsync();
                List<GetDeliveryListCourier> ProcessedList = _mapper.Map<List<Delivery>, List<GetDeliveryListCourier>>(DeliveryList);
                return new ObjectResult(ProcessedList)
                {
                    StatusCode = 200
                };
            } catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException.Message);
            }
        }
        [HttpGet("current_delivery"), Authorize]
        public async Task<IActionResult> GetCurrentDeliveryCourier()
        {
            try
            {
                var courier_id = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                var deliveries = await _context.Delivery
                    .Include(d => d.SenderPool)
                    .Include(d => d.ReceiverPool)
                    .Include(d => d.Messages)
                    .Where(d => d.courier_id == courier_id).FirstOrDefaultAsync();
                if (deliveries == null) return NotFound(new { message = "No delivery!" });
                GetDeliveryResult processedDeliveryObject = _mapper.Map<Delivery, GetDeliveryResult>(deliveries);


                //var result = deliveries.ReceiverPool.pool_phone
                return Ok(processedDeliveryObject);
            }
            catch (Exception ex)
            {
                return StatusCode(404, ex);
            }
        }

        [HttpPost("request_delivery"), Authorize]
        public async Task<IActionResult> RequestDelivery()
        {
            try
            {
                var courier_id = new Guid(User?.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                var courier_username = User?.Identity?.Name;

                var courier_data = await _context.Courier.Where(c => c.courier_id == courier_id).FirstOrDefaultAsync();

                // fetch all deliveries that are on pool.
                var deliveriesOnPool = await _context.Delivery.Where(d => d.delivery_status == "on_destination_pool").ToListAsync();

                if (deliveriesOnPool.Count == 0) return NotFound(new { message = "There are no pending deliveries at the moment!" });

                var prioDeliveries = deliveriesOnPool.Where(prio => prio.service_type == "PRIO");
                var regDeliveries = deliveriesOnPool.Where(prio => prio.service_type == "REG");
                var combinedPrioritizedDeliveries = prioDeliveries.Concat(regDeliveries).ToList();

                //return Ok(new {combinedPrioritizedDeliveries,  availableCouriers});
                //return Ok(availableCouriers);
                // if deliveries is smaller than number of couriers end loop;
                // if deliveries is more than number of couriers, continue.
                var delivery = combinedPrioritizedDeliveries[0];
                delivery.courier_id = courier_id;
                delivery.delivery_status = "otw_receiver_address";
                courier_data.courier_availability = false;
                var message = new Message()
                {
                    tracking_number = delivery.tracking_number,
                    message_text = $"Package is with courier {courier_username} and is on the way to {delivery.receiver_address}",
                    timestamp = DateTime.Now,
                };
                await _context.Message.AddAsync(message);
                await _context.SaveChangesAsync();

                GetDeliveryResult responseDelivery = _mapper.Map<GetDeliveryResult>(delivery);

                return Ok(responseDelivery);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException.Message);
            }

    }

}
        //[HttpPost("Cancel")]
        //public async Task<IActionResult> CancelRequest([FromBody] GetCancelRequest cancel)
        //{
        //    var trackingExist = await _context.Delivery.Where(d => d.tracking_number == cancel.tracking_number).ToListAsync();
        //    if (trackingExist.Count == 1)
        //    {
        //        trackingExist.return_status = true;
        //        await _context.SaveChangesAsync();
        //    }

        //}
    
}
