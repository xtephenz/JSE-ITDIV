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

namespace JSE.Controllers
{
    [Route("auth/admin")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration, AppDbContext context)

        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] AdminRegisterRequest register)
        {
            var CheckPoolCity = await _context.PoolBranch.Where(c => c.pool_city == register.pool_city).ToListAsync();
            if (register.pool_city == "" || CheckPoolCity.Count == 0) return new ObjectResult(new { message = "Invalid pool city!" });
            if (register.admin_password != register.admin_confirm_password) return new ObjectResult(new { message = "Password mismatch!" }) 
                                                                                       { StatusCode = 500 };
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
                    StatusCode = 500
                };
            }

        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] AdminLoginRequest login)
        {
            var CheckUser = await _context.Admin.Where(c => c.admin_username == login.admin_username).ToListAsync();
            if (CheckUser.Count > 0)
            {
                var PasswordCheck = BCrypt.Net.BCrypt.EnhancedVerify(login.admin_password, CheckUser[0].admin_password);
                // var PasswordCheck = login.admin_password == CheckUser[0].admin_password;
                if (PasswordCheck)
                {
                    var token = CreateToken(login.admin_username, CheckUser[0].admin_id);
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
        private string CreateToken(String Email, Guid UserId)
        {
            List<Claim> claims = new List<Claim> {
                new Claim("admin_username", Email.ToString()),
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
