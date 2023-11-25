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

namespace JSE.Controllers
{
    [Route("auth/courier")]
    [ApiController]
    public class CourierController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public CourierController(IConfiguration configuration, AppDbContext context)

        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] CourierRegisterRequest register)
        {
            if (register.courier_password != register.courier_confirm_password) return new ObjectResult(new { message = "Password mismatch!" })
            { StatusCode = 500 };
            if (register.courier_password != register.courier_confirm_password) return new ObjectResult(new { message = "Password mismatch!" }) 
                                                                                       { StatusCode =400 };
            var userExists = await _context.Courier.Where(c => c.courier_username == register.courier_username).ToListAsync();
            if (userExists.Count == 0)
            {
                register.courier_password = BCrypt.Net.BCrypt.EnhancedHashPassword(register.courier_password, 13);
                var newCourrier = new Courier()
                {
                    courier_username = register.courier_username,
                    courier_password = register.courier_password,
                };
                _context.Courier.Add(newCourrier);
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
