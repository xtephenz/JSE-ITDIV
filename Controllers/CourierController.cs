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

        /// <remarks>
        /// Sample result:
        /// 
        ///     GET
        ///     [
        ///         {
        ///             "courier_username": "kyuri",
        ///             "courier_phone": "111111",
        ///             "courier_availability": true
        ///         }
        ///     ]
        /// </remarks>
        /// <param name="all"></param>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCouriers()
        {
            var couriers = await _context.Courier.ProjectTo<GetCourierResult>(_mapper.ConfigurationProvider).ToListAsync();
            return Ok(couriers);
        }

        /// <remarks>
        /// Sample result:
        /// 
        ///     GET
        ///         {
        ///             "courier_username": "kyuri",
        ///             "courier_phone": "123123123132",
        ///             "pool_city": "Depok",
        ///             "courier_availability": false
        ///         }
        /// </remarks>
        /// <param name="profile"></param>
        [HttpGet("profile"), Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            var courier_id = new Guid(User?.FindFirstValue(ClaimTypes.NameIdentifier).ToString()); //? bisa ada value bisa tidak
            var courier_data = await _context.Courier.Where(c => c.courier_id == courier_id).FirstOrDefaultAsync();

            var courier_profile = _mapper.Map<GetCourierResult>(courier_data);
            return Ok(courier_profile);
        }
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST
        ///     {
        ///         "courier_username": "kyuri",
        ///         "courier_password": "Wra@ck$",
        ///         "courier_confirm_password": "Wra@ck$",
        ///         "pool_city": "Tangerang", (pilihan: "Jakarta"/ "Tangerang"/ "Depok"/ "Bogor"),
        ///         "courier_phone": "121212"
        ///     }
        /// </remarks>
        /// <param name="register"></param>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] CourierRegisterRequest register)
        {
            var CheckPoolCity = await _context.PoolBranch.Where(c => c.pool_name == register.pool_city).ToListAsync();
            if (register.pool_city == "" || CheckPoolCity.Count == 0)
                return BadRequest(new { message = "Invalid pool city!" });

            if (register.courier_password != register.courier_confirm_password) return BadRequest(new { message = "Password mismatch!" });

            var userExists = await _context.Courier.Where(c => c.courier_username == register.courier_username).ToListAsync();
            if (userExists.Count == 0)
            {
                register.courier_password = BCrypt.Net.BCrypt.EnhancedHashPassword(register.courier_password, 13);
                var newCourier = new Courier()
                {
                    courier_username = register.courier_username,
                    courier_password = register.courier_password,
                    pool_city = register.pool_city,
                    courier_phone = register.courier_phone,
                };
                _context.Courier.Add(newCourier);
                await _context.SaveChangesAsync();
                return Ok(register);
            }
            else
            {
                return BadRequest(new { message = "Username already exists!" });
            }

        }
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST
        ///     {
        ///         "courier_username": "kyuri",
        ///         "courier_password": "Wra@ck$"
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
                    var token = CreateToken(login.courier_username, CheckUser.courier_id, CheckUser.pool_city);
                    var responseData = new { token = token };
                    return Ok(responseData);
                }
                else
                {
                    var responseData = new { message = "Login Unsuccessful, email or password invalid!" };
                    return StatusCode(401, responseData);
                }
            }
            else
            {
                var responseData = new { message = "Login Unsuccessful, email or password invalid!" };
                return StatusCode(401, responseData);
            }
        }
        private string CreateToken(String username, Guid UserId, String pool_city)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, username.ToString()),
                new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
                new Claim("pool_city", pool_city.ToString()),
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


        /// <remarks>
        /// Sample result:
        /// 
        ///     GET
        ///         [
        ///           {
        ///             "tracking_number": "PRIO01122300004",
        ///             "delivery_status": "package_delivered",
        ///             "actual_receiver_name": "satpam",
        ///             "receiver_phone": "22222",
        ///             "receiver_address": "anggrek 111",
        ///             "courier_id": "8d4920ee-be9f-4c22-ceab-08dbf29e8e5a",
        ///             "fail_message": null,
        ///             "arrival_date": "2023-12-02T15:14:25.812689",
        ///             "pool_receiver_city": "Tangerang"
        ///           },
        ///           {
        ///             "tracking_number": "PRIO01122300005",
        ///             "delivery_status": "otw_receiver_address",
        ///             "actual_receiver_name": null,
        ///             "receiver_phone": "33333",
        ///             "receiver_address": "anggrek 333",
        ///             "courier_id": "8d4920ee-be9f-4c22-ceab-08dbf29e8e5a",
        ///             "fail_message": null,
        ///             "arrival_date": "0001-01-01T00:00:00",
        ///             "pool_receiver_city": "Bekasi"
        ///           }
        ///         ]
        /// </remarks>
        /// <params name="history"></params>

        [HttpGet("history"), Authorize]

        public async Task<IActionResult> GetDeliveryListCourier()
        {
            try
            {
                var accepted_statuses = new List<String> { "package_delivered", "delivery_failed", "returned_to_pool" };
                var courier_id = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                var DeliveryList = await _context.Delivery.Where(c => c.courier_id == courier_id && accepted_statuses.Contains(c.delivery_status)).ToListAsync();
                DeliveryList.Reverse();
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
       
        /// <remarks>
        /// Sample result:
        /// 
        ///     GET
        ///         {
        ///            "tracking_number": "PRIO01122300004",
        ///            "sending_date": "2023-12-01T14:14:33.673",
        ///            "sender_name": "sender dua",
        ///            "sender_phone": "11111",
        ///            "sender_address": "binus 111",
        ///            "intended_receiver_name": "receiver satu",
        ///            "receiver_phone": "22222",
        ///            "receiver_address": "anggrek 111",
        ///            "service_type": "PRIO",
        ///            "package_weight": 1,
        ///            "delivery_price": 10000,
        ///            "delivery_status": "otw_receiver_address",
        ///            "actual_receiver_name": null,
        ///            "courier_id": "8d4920ee-be9f-4c22-ceab-08dbf29e8e5a",
        ///            "Courier": null,
        ///            "arrival_date": null,
        ///            "returned_status": null,
        ///            "fail_message": null,
        ///            "pool_sender_city": "Jakarta",
        ///            "SenderPool": {
        ///                      "pool_name": "Jakarta",
        ///                      "pool_phone": "11111"
        ///                  },
        ///            "pool_receiver_city": "Tangerang",
        ///            "ReceiverPool": {
        ///                      "pool_name": "Tangerang",
        ///                      "pool_phone": "22222"
        ///                  },
        ///            "Messages": [
        ///                     {
        ///                         "tracking_number": "PRIO01122300004",
        ///                         "message_text": "Package received at Jakarta pool.",
        ///                         "timestamp": "2023-12-02T15:05:50.679964"
        ///                     },
        ///                     {
        ///                         "tracking_number": "PRIO01122300004",
        ///                         "message_text": "Package is on the way to Tangerang pool.",
        ///                         "timestamp": "2023-12-02T15:06:19.079793"
        ///                     },
        ///                     {
        ///                         "tracking_number": "PRIO01122300004",
        ///                         "message_text": "Package has arrived at Tangerang pool.",
        ///                         "timestamp": "2023-12-02T15:06:29.833395"
        ///                     },
        ///                     {
        ///                         "tracking_number": "PRIO01122300004",
        ///                         "message_text": "Package is with courier kyuri and is on the way to anggrek 111",
        ///                         "timestamp": "2023-12-02T15:06:37.134299"
        ///                     }
        ///                  ]
        ///         }
        /// </remarks>
        /// <param name="current_delivery"></param>
        [HttpGet("current_delivery"), Authorize]
        public async Task<IActionResult> GetCurrentDeliveryCourier()
        {
            try
            {
                List<String> accepted_statuses = new List<String> { "on_destination_pool", "otw_receiver_address", "package_delivered", "delivery_failed" };
                var courier_id = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                var deliveries = await _context.Delivery
                    .Include(d => d.SenderPool)
                    .Include(d => d.ReceiverPool)
                    .Include(d => d.Messages)
                    .Where(d => d.courier_id == courier_id && accepted_statuses.Contains(d.delivery_status)).FirstOrDefaultAsync();
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

        [HttpPost("request_delivery")]
        [Authorize(Roles = "Courier")]
        public async Task<IActionResult> RequestDelivery()
        {
            try
            {
                var courier_id = new Guid(User?.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                var courier_username = User?.Identity?.Name;
                var courier_pool_city = User?.FindFirstValue("pool_city").ToString();

                var courier_data = await _context.Courier.Where(c => c.courier_id == courier_id).FirstOrDefaultAsync();

                if (courier_data.courier_availability == false) return BadRequest(new { message = "You have a ongoing delivery job! Finish the ongoing delivery and try again." });

                // fetch all deliveries that are on pool.
                var deliveriesOnPool = await _context.Delivery.Where(d => d.delivery_status == "on_destination_pool" && d.pool_receiver_city == courier_pool_city).ToListAsync();

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
