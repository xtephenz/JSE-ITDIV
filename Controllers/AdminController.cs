using JSE.Data;
using JSE.Models;
using JSE.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt;
using Microsoft.Extensions.Hosting;

namespace JSE.Controllers
{
    [Route("auth/admin")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration; // untuk web token

        public AdminController(IConfiguration configuration, AppDbContext context)

        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] AdminRegisterRequest register)
        {
            var CheckPoolCity = await _context.PoolBranch.Where(c => c.pool_name == register.pool_city).ToListAsync();
            if (register.pool_city == "" || CheckPoolCity.Count == 0)
                return new ObjectResult(new { message = "Invalid pool city!" });

            if (register.admin_password != register.admin_confirm_password)
                return new ObjectResult(new { message = "Password mismatch!" }){ StatusCode = 401 };

            var userExists = await _context.Admin.Where(c => c.admin_username == register.admin_username).ToListAsync();
            if (userExists.Count == 0)
            {
                register.admin_password = BCrypt.Net.BCrypt.EnhancedHashPassword(register.admin_password, 13);
                var admin = new Admin()
                {
                    admin_username = register.admin_username,
                    admin_password = register.admin_password,
                    pool_city = register.pool_city
                };
                _context.Admin.Add(admin);
                await _context.SaveChangesAsync();
                return Ok(register);
            }
            else
            {
                return new ObjectResult(new {message = "Username already exists!"})
                {
                    StatusCode = 400
                };
            }

        }
        

        
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] AdminLoginRequest login)
        {
            var CheckUser = await _context.Admin.Where(c => c.admin_username == login.admin_username).FirstOrDefaultAsync();
            if (CheckUser != null)
            {
                var PasswordCheck = BCrypt.Net.BCrypt.EnhancedVerify(login.admin_password, CheckUser.admin_password);
                // var PasswordCheck = login.admin_password == CheckUser[0].admin_password;
                if (PasswordCheck)
                {
                    var token = CreateToken(login.admin_username, CheckUser.admin_id);
                    var responseData = new { token = token };
                    return new ObjectResult(responseData) {
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

        //[HttpPost("receiveImageFromCourier")]
        //public IActionResult ReceiveImageFromCourier(IFormFile image, string trackingNumber)
        //{
        //    if (image == null || image.Length == 0)
        //        return BadRequest("Invalid image file");

        //    var delivery = _context.Delivery.Find(trackingNumber);

        //    if (delivery == null)
        //        return NotFound("Delivery not found");

        //    var imagePath = SaveImage(image);

        //    delivery.imagePath = imagePath;
        //    _context.SaveChanges();

        //    return Ok("Image received successfully");
        //}

        private string SaveImage(IFormFile image)
        {
            var uniqueFileName = $"{Guid.NewGuid().ToString()}_{image.FileName}";
            var filePath = Path.Combine("images", uniqueFileName); // Path.Combine untuk membangun path file

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            return uniqueFileName;
        }
        private string CreateToken(String Username, Guid UserId)
        {
            List<Claim> claims = new List<Claim> {
                new Claim("admin_username", Username.ToString()),
                new Claim("admin_id", UserId.ToString()),
                new Claim(ClaimTypes.Role, "Admin"),
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
    }
}
