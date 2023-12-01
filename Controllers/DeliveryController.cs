using AutoMapper;
using AutoMapper.QueryableExtensions;
using JSE.Data;
using JSE.Models;
using JSE.Models.Requests;
using JSE.Models.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Nodes;

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
        [HttpGet("admin_pool"), Authorize] 
        public async Task<IActionResult> GetDeliveries()
        {
            try
            {
                // get admin pool_city
                var adminPoolCity = User.FindFirstValue("pool_city").ToString();
                var deliveries = await _context.Delivery
                    .Include(d => d.SenderPool)
                    .Include(d => d.ReceiverPool)
                    .Include(d => d.Messages)
                    .Include(d => d.Courier)
                    .Where(d => d.pool_sender_city ==  adminPoolCity || d.pool_receiver_city == adminPoolCity)
                    .ProjectTo<GetDeliveryResult>(_mapper.ConfigurationProvider)
                    .ToListAsync();
                return Ok(deliveries);
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
                return StatusCode(404, ex);
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


                var message = new Message()
                {
                    tracking_number = trackingNumber,
                    message_text = $"Package received at {delivery.pool_sender_city} pool.",
                    timestamp = DateTime.Now
                };
                Delivery processedDeliveryObject = _mapper.Map<CreateDelivery, Delivery>(delivery);

                processedDeliveryObject.tracking_number = trackingNumber;
                processedDeliveryObject.delivery_status = "on_sender_pool";

                await _context.Message.AddAsync(message);
                await _context.Delivery.AddAsync(processedDeliveryObject);
                await _context.SaveChangesAsync();

                GetDeliveryResult output = _mapper.Map<Delivery, GetDeliveryResult>(processedDeliveryObject);
                return Ok(output);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("/delivery/{tracking_number}")]
        public async Task<IActionResult> GetByTrackingNumber(String tracking_number) //FromBody itu json
        {
            try
            {
                var deliveries = await _context.Delivery
                    .Include(d => d.SenderPool)
                    .Include(d => d.ReceiverPool)
                    .Include(d => d.Messages)
                    .Include(d => d.Courier)
                    .Where(d => d.tracking_number == tracking_number)
                    .ProjectTo<GetDeliveryResult>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
                //GetDeliveryResult processedDeliveryObject = _mapper.Map<Delivery, GetDeliveryResult>(deliveries);


                //var result = deliveries.ReceiverPool.pool_phone
                return Ok(deliveries);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPatch("/dispatch")] //admin
        public async Task<IActionResult> NewDispatchToDestPool(String tracking_number)
        {
            try
            {
                var delivery = await _context.Delivery.FindAsync(tracking_number);
                if (delivery.delivery_status == "on_sender_pool")
                {
                    delivery.delivery_status = "dispatched";
                    var newMessage = new Message()
                    {
                        message_text = $"Package is on the way to {delivery.pool_receiver_city} pool.",
                        tracking_number = tracking_number,
                        timestamp = DateTime.Now,
                    };

                GetMessageResult result = _mapper.Map<Message, GetMessageResult>(newMessage);
                await _context.Message.AddAsync(newMessage);
                await _context.SaveChangesAsync();
                return Ok(result);
                }
                else
                {
                    return BadRequest($"Invalid request! package is already on status: {delivery.delivery_status}.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpPatch("/arrived")] //admin
        public async Task<IActionResult> NewArrivalAtDestPool(String tracking_number)
        {
            try
            {
                var delivery = await _context.Delivery.FindAsync(tracking_number);
                if (delivery.delivery_status == "dispatched")
                {
                    delivery.delivery_status = "on_destination_pool";
                    var newMessage = new Message()
                    {
                        message_text = $"Package has arrived at {delivery.pool_receiver_city} pool.",
                        tracking_number = tracking_number,
                        timestamp = DateTime.Now,
                    };

                    GetMessageResult result = _mapper.Map<Message, GetMessageResult>(newMessage);
                    await _context.Message.AddAsync(newMessage);
                    await _context.SaveChangesAsync();
                    return Ok(result);
                }
                else
                {
                    return BadRequest($"Invalid request!, package is already on status: {delivery.delivery_status}.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        /*[HttpPatch("/toReceiverAddress")] */// auto assign on arrive -> prio and reg services becomes obsolete. have to revise.
        //public async Task<IActionResult> NewDispatchToReceiverAddr(String tracking_number)
        //{
        //    try
        //    {
        //        var available_courier = await _context.Courier.Where(c => c.courier_availability == true).FirstAsync();

        //        var delivery = await _context.Delivery.FindAsync(tracking_number);
        //        if (delivery.delivery_status == "on_destination_pool")
        //        {
        //            delivery.delivery_status = "otw_receiver_address";
        //            available_courier.courier_availability = false;
        //            delivery.courier_id = available_courier.courier_id;

        //            var newMessage = new Message()
        //            {
        //                message_text = $"Your package is with courier {available_courier.courier_username} and is on the way to {delivery.receiver_address}.",
        //                tracking_number = tracking_number,
        //                timestamp = DateTime.Now,
        //            };

        //            GetMessageResult result = _mapper.Map<Message, GetMessageResult>(newMessage);
        //            await _context.Message.AddAsync(newMessage);
        //            await _context.SaveChangesAsync();
        //            return Ok(result);
        //        }
        //        else
        //        {
        //            return BadRequest($"Invalid request!, package is already on status: {delivery.delivery_status}.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}
        [HttpPost("/assignDeliveries")] //admin pencetd
        public async Task<IActionResult> AssignDeliveriesToCourier()
        {
            try
            {
                // fetch all deliveries that are on pool.
                var deliveriesOnPool = await _context.Delivery.Where(d => d.delivery_status == "on_destination_pool").ToListAsync();
                var prioDeliveries = deliveriesOnPool.Where(prio => prio.service_type == "PRIO");
                var regDeliveries = deliveriesOnPool.Where(prio => prio.service_type == "REG");
                var combinedPrioritizedDeliveries = prioDeliveries.Concat(regDeliveries).ToList();

                var availableCouriers = await _context.Courier.Where(d => d.courier_availability == true).ToListAsync();
                //return Ok(new {combinedPrioritizedDeliveries,  availableCouriers});
                //return Ok(availableCouriers);
                for (int i = 0; i < availableCouriers.Count; i++)
                {
                    // if deliveries is smaller than number of couriers end loop;
                    if (combinedPrioritizedDeliveries.Count == i) break;
                    // if deliveries is more than number of couriers, continue.
                    try
                    {
                        var delivery = combinedPrioritizedDeliveries[i];
                        var courier = availableCouriers[i];
                        delivery.courier_id = availableCouriers[i].courier_id;
                        delivery.delivery_status = "otw_receiver_address";
                        availableCouriers[i].courier_availability = false;
                        var message = new Message()
                        {
                            tracking_number = delivery.tracking_number,
                            message_text = $"Package is with courier {courier.courier_username} and is on the way to {delivery.receiver_address}",
                            timestamp = DateTime.Now,
                        };
                        await _context.Message.AddAsync(message);
                    }
                    catch (Exception exc)
                    {
                        continue;
                    }
                }
                await _context.SaveChangesAsync();

                return Ok("Deliveries assigned to courier!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
        [HttpPatch("/successDelivery"), Authorize] // COURIER PENCET
        public async Task<IActionResult> SuccessDelivery(String tracking_number, String receiver_name)
        {
            try
            {
                var delivery = await _context.Delivery.Include(d => d.Courier).Where(d => d.tracking_number == tracking_number).FirstAsync();
                if (delivery.delivery_status == "otw_receiver_address")
                {
                    delivery.delivery_status = "package_delivered";
                    delivery.actual_receiver_name = receiver_name;
                    delivery.Courier.courier_availability = true;
                    delivery.arrival_date = DateTime.Now;
                    var newMessage = new Message()
                    {
                        message_text = $"Package is received by {receiver_name}.",
                        tracking_number = tracking_number,
                        timestamp = DateTime.Now,
                    };

                    GetMessageResult result = _mapper.Map<Message, GetMessageResult>(newMessage);
                    await _context.Message.AddAsync(newMessage);
                    await _context.SaveChangesAsync();
                    return Ok(result);
                }
                else
                {
                    return BadRequest($"Invalid request!, package is already on status: {delivery.delivery_status}.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        //courrier
        [HttpPatch("/failedDelivery")]
        public async Task<IActionResult> FailedDelivery(String tracking_number, String courier_message)
        {
            try
            {
                var delivery = await _context.Delivery.FindAsync(tracking_number);
                if (delivery.delivery_status == "otw_receiver_address")
                {
                    delivery.delivery_status = "delivery_failed";
                    var newMessage = new Message()
                    {
                        message_text = $"Package is rejected. \"{courier_message}\"",
                        tracking_number = tracking_number,
                        timestamp = DateTime.Now,
                    };

                    GetMessageResult result = _mapper.Map<Message, GetMessageResult>(newMessage);
                    await _context.Message.AddAsync(newMessage);
                    await _context.SaveChangesAsync();
                    return Ok(result);
                }
                else
                {
                    return BadRequest($"Invalid request! package is already on status: {delivery.delivery_status}.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        
    }
}





