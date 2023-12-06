using JSE.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JSE.Controllers
{
    [Route("image")]
    [ApiController]
    public class ImageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration; // untuk web token

        public ImageController(IConfiguration configuration, AppDbContext context)

        {
            _configuration = configuration;
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> ReceiveImageFromCourier(IFormFile image, string trackingNumber)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Invalid image file");

            var delivery = _context.Delivery.Find(trackingNumber);

            if (delivery == null)
                return NotFound("Delivery not found");

            var imagePath = SaveImage(image);

            delivery.image_path = imagePath;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Image received successfully!" });
        }

        private string SaveImage(IFormFile image)
        {
            var uniqueFileName = $"{Guid.NewGuid().ToString()}_{image.FileName}";
            var directoryPath = Path.Combine("wwwroot", "images"); // Adjust the path as needed
            var filePath = Path.Combine(directoryPath, uniqueFileName);

            // Ensure the directory exists; if not, create it
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            return uniqueFileName;
        }

        [HttpGet("{tracking_number}")]
        public async Task<IActionResult> GetImage(string tracking_number)
        {
            var delivery = await _context.Delivery.Where(c => c.tracking_number == tracking_number).FirstOrDefaultAsync();
            var image_absolute_path = delivery.image_path;
            if (delivery == null) return NotFound(new { message = "Tracking number is invalid!" });
            if (image_absolute_path == null) return NotFound(new { message = "No image found!" });
            var directoryPath = Path.Combine("wwwroot", "images"); // Adjust the path as needed
            var filePath = Path.Combine(directoryPath, image_absolute_path);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "Image doesn't exist!" });
            }

            var imageBytes = System.IO.File.ReadAllBytes(filePath);

            // Determine the content type based on the file extension or your specific requirements
            var contentType = "image/jpeg"; // Change to match your image type

            return File(imageBytes, contentType);
        }
    }
}
