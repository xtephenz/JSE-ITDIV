using AutoMapper;
using AutoMapper.QueryableExtensions;
using JSE.Data;
using JSE.Models;
using JSE.Models.Requests;
using JSE.Models.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JSE.Controllers
{
    [Route("delivery")]
    [ApiController]

    public class DeliveryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public DeliveryController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        //[HttpGet("/daftar-pesanan"), Authorize(Roles = "Admin")]
        [HttpGet("get-by-admin-id")]
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
        [HttpGet("/delivery")]
        public async Task<IActionResult> GetAllDeliveries()
        {
            try
            {
                var deliveries = await _context.Delivery
                    .ProjectTo<GetDeliveryResult>(_mapper.ConfigurationProvider)
                    .ToListAsync();
                //var result = deliveries.ReceiverPool.pool_phone
                return Ok(deliveries);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("/delivery")]
        public async Task<IActionResult> PostDelivery([FromBody] CreateDelivery delivery)
        {
            try
            {
                // create tracking number:
                /*
                Tracking Number Format: XYZ-DDMMYY-12345

                In this modified format:

                Service Type (XYZ): This part represents the type of service.
                Shipment Date (DDMMYY): This part encodes the date of shipment or order placement, using a date format like YYMMDD or MMDDYY.
                Package Identifier (12345): This part is a unique identifier for the package, allowing for a larger range of possibilities.
                */
                string packageType = delivery.service_type.ToString();
                int packagesToDate =  _context.Delivery.Where(d => d.sending_date == delivery.sending_date).Count() + 1;
                string packageIdentifier = packagesToDate.ToString("D5");
                string shipmentDate = delivery.sending_date.ToString("ddMMyy");
                string trackingNumber = $"{packageType}{shipmentDate}{packageIdentifier}";

                delivery.tracking_number = trackingNumber;
                delivery.delivery_status = "on_process";

                var message = new GetMessageResult()
                {
                    tracking_number = trackingNumber,
                    message_text = $"Package received at {delivery.pool_sender_city} pool.",
                    timestamp = DateTime.Now
                };
                Delivery processedDeliveryObject = _mapper.Map<CreateDelivery, Delivery>(delivery);

                Message processedMessageObject = _mapper.Map<GetMessageResult, Message>(message);
                _context.Message.Add(processedMessageObject);
                _context.Delivery.Add(processedDeliveryObject);
                await _context.SaveChangesAsync();
                return Ok(delivery);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

    }
}





