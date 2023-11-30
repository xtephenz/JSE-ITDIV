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
using Microsoft.Extensions.Hosting;


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
            var couriers = await _context.Courier.ToListAsync();
            return Ok(couriers);
        }

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

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] CourierLoginRequest login)
        {
            var CheckUser = await _context.Courier.Where(c => c.courier_username == login.courier_username).ToListAsync();
            if (CheckUser.Count > 0)
            {
                var PasswordCheck = BCrypt.Net.BCrypt.EnhancedVerify(login.courier_password, CheckUser[0].courier_password);
                // var PasswordCheck = login.admin_password == CheckUser[0].admin_password;
                if (PasswordCheck)
                {
                    var token = CreateToken(login.courier_username, CheckUser[0].courier_id);
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

        [HttpPost("sendImage")]
        public IActionResult SendImage(IFormFile image, string trackingNumber)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Invalid image file");

            var delivery = _context.Delivery.Find(trackingNumber);

            if (delivery == null)
                return NotFound("Delivery not found");

            var imagePath = SaveImage(image);

            delivery.imagePath = imagePath;
            _context.SaveChanges();

            return Ok("Image sent successfully");
        }

        private string SaveImage(IFormFile image)
        {
            var uniqueFileName = $"{Guid.NewGuid().ToString()}_{image.FileName}";
            var filePath = Path.Combine("images", uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            return uniqueFileName;
        }



        private string CreateToken(String username, Guid UserId)
        {
            List<Claim> claims = new List<Claim> {
                new Claim("courier_username", username.ToString()),
                new Claim("courier_id", UserId.ToString()),
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
        [HttpGet("history")]
        public async Task<IActionResult> GetDeliveryListCourier(Guid courier_id)
        {
            try
            {
                var DeliveryList = await _context.Delivery.Where(c => c.courier_id == courier_id).ToListAsync();
                List<GetDeliveryListCourier> ProcessedList = _mapper.Map<List<Delivery>, List<GetDeliveryListCourier>>(DeliveryList);
                return new ObjectResult(ProcessedList)
                {
                    StatusCode = 200
                };
            } catch (Exception ex)
            {
                return StatusCode(500, ex);
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
}
