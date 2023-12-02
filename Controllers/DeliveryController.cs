using AutoMapper;
using AutoMapper.QueryableExtensions;
using JSE.Data;
using JSE.Models;
using JSE.Models.Requests;
using JSE.Models.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
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

        /// <remarks>
        /// Sample request:
        /// 
        ///     GET
        ///     {
        ///         "tracking_number": "reg01122300001",
        ///         "sending_date": "2023-12-01T14:14:33.673",
        ///         "sender_name": "sender satu",
        ///         "sender_phone": "11111",
        ///         "sender_address": "binus 111",
        ///         "intended_receiver_name": "receiver satu",
        ///         "receiver_phone": "22222",
        ///         "receiver_address": "anggrek 111",
        ///         "service_type": "REG",
        ///         "package_weight": 1,
        ///         "delivery_price": 10000,
        ///         "delivery_status": "on_sender_pool",
        ///         "actual_receiver_name": null,
        ///         "courier_id": null,
        ///         "Courier": null,
        ///         "arrival_date": null,
        ///         "returned_status": null,
        ///         "fail_message": null,
        ///         "pool_sender_city": "Jakarta",
        ///         "SenderPool":
        ///             {
        ///                 "pool_name": "Jakarta",
        ///                 "pool_phone": "11111"
        ///             },
        ///         "pool_receiver_city": "tangerang",
        ///         "ReceiverPool":
        ///             {
        ///                 "pool_name": "Tangerang",
        ///                 "pool_phone": "22222"
        ///             },
        ///         "Messages": [
        ///             {
        ///                 "tracking_number": "reg01122300001",
        ///                 "message_text": "Package received at jakarta pool.",
        ///                 "timestamp": "2023-12-02T00:37:59.737891"
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <param name="admin_pool"></param>
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

                deliveries.Reverse();
                return Ok(deliveries);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.Message);;
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
                deliveries.Reverse();
                return Ok(deliveries);

            }
            catch (Exception ex)
            {
                return StatusCode(404, ex);
            }
        }
        ///<remarks>
        ///Sample request:
        ///
        ///     POST
        ///     {
        ///         "sender_name": "sender satu",
        ///         "sender_phone": "11111",
        ///         "sender_address": "binus 111",
        ///         "intended_receiver_name": "receiver satu",
        ///         "receiver_phone": "22222",
        ///         "receiver_address": "anggrek 111",
        ///         "service_type": "REG",
        ///         "package_weight": 1,
        ///         "delivery_price": 10000,
        ///         "pool_sender_city": "Jakarta",
        ///         "pool_receiver_city": "Tangerang"
        ///     }
        ///</remarks>
        ///
        /// <param name="login"></param>
        [HttpPost("/create_delivery")]
        public async Task<IActionResult> PostDelivery([FromBody] CreateDelivery delivery)
        {
            try
            {
                // create tracking number:
                /*
                Tracking Number Format: XYZ-YYYYMMDD-12345

                In this modified format:

                Service Type (XYZ): This part represents the type of service.
                Shipment Date (DDMMYY): This part encodes the date of shipment or order placement, using a date format like YYMMDD or MMDDYY.
                Package Identifier (12345): This part is a unique identifier for the package, allowing for a larger range of possibilities.
                */
                DateTime sending_date = DateTime.Now;
                string packageType = delivery.service_type.ToString().ToUpper();
                int packagesToDate =  _context.Delivery.Where(d => d.sending_date == sending_date).Count() + 1;
                string packageIdentifier = packagesToDate.ToString("D5");
                string shipmentDate = sending_date.ToString("yyyyMMdd");
                string trackingNumber = $"{packageType}{shipmentDate}{packageIdentifier}";


                var message = new Message()
                {
                    tracking_number = trackingNumber,
                    message_text = $"Package received at {delivery.pool_sender_city} pool.",
                    timestamp = DateTime.Now
                };
                Delivery processedDeliveryObject = _mapper.Map<CreateDelivery, Delivery>(delivery);
                //Console.WriteLine(processedDeliveryObject);
                processedDeliveryObject.tracking_number = trackingNumber;
                processedDeliveryObject.delivery_status = "on_sender_pool";


                var output = _mapper.Map<GetDeliveryResult>(processedDeliveryObject);

                

                await _context.Message.AddAsync(message);
                await _context.Delivery.AddAsync(processedDeliveryObject);
                await _context.SaveChangesAsync();

                return Ok(output);

            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.Message);
            }
        }
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET
        ///     {
        ///         "tracking_number": "PRIO01122300001",
        ///         "sending_date": "2023-12-01T14:14:33.673",
        ///         "sender_name": "sender satu",
        ///         "sender_phone": "11111",
        ///         "sender_address": "binus 111",
        ///         "intended_receiver_name": "receiver satu",
        ///         "receiver_phone": "22222",
        ///         "receiver_address": "anggrek 111",
        ///         "service_type": "PRIO", ->param: "REG" atau "PRIO"
        ///         "package_weight": 1,
        ///         "delivery_price": 10000,
        ///         "delivery_status": "on_sender_pool",
        ///         "actual_receiver_name": null,
        ///         "courier_id": null,
        ///         "Courier": null,
        ///         "arrival_date": null,
        ///         "returned_status": null,
        ///         "fail_message": null,
        ///         "pool_sender_city": "Jakarta",
        ///         "SenderPool":
        ///             {
        ///               "pool_name": "Jakarta",
        ///               "pool_phone": "11111"
        ///             },
        ///         "pool_receiver_city": "Tangerang",
        ///         "ReceiverPool":
        ///             {
        ///               "pool_name": "Tangerang",
        ///               "pool_phone": "22222"
        ///             },
        ///         "Messages": [
        ///             {
        ///               "tracking_number": "reg01122300001",
        ///               "message_text": "Package received at Jakarta pool.",
        ///               "timestamp": "2023-12-02T00:37:59.737891"
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <param name="tracking_number"></param>

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
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.Message);;
            }
        }

        /// <remarks>
        /// Sample request:
        /// 
        ///     PATCH
        ///     {
        ///         "tracking_number": "PRIO01122300004",
        ///         "message_text": "Package is on the way to Tangerang pool.",
        ///         "timestamp": "2023-12-02T15:06:19.079793+07:00"
        ///     }
        /// </remarks>
        /// <param name="dispatch"></param>
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
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.Message);;
            }
        }
        /// <remarks>
        /// Sample request:
        /// 
        ///     PATCH
        ///     {
        ///       "tracking_number": "PRIO01122300004",
        ///       "message_text": "Package has arrived at Tangerang pool.",
        ///       "timestamp": "2023-12-02T15:06:29.833395+07:00"
        ///     }
        /// </remarks>
        /// <param name="arrived"></param>
        [HttpPatch("/arrived"), Authorize] //admin
        public async Task<IActionResult> NewArrivalAtDestPool(String tracking_number)
        {
            try
            {
                var delivery = await _context.Delivery.FindAsync(tracking_number);
                var admin_pool_city = User?.FindFirstValue("pool_city").ToString();
                if (admin_pool_city != delivery.pool_receiver_city) 
                    return BadRequest($"You are not the admin of {delivery.pool_receiver_city}! Please use the correct credentials to update package status!");
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
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.Message);;
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
        //        return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.Message);;
        //    }
        //}

        /// <remarks>
        /// Sample request:
        /// 
        ///     Deliveries assigned to courier!
        /// </remarks>
        /// <param name="arrived"></param>
        [HttpPost("/assignDeliveries"), Authorize] //admin pencet
        public async Task<IActionResult> AssignDeliveriesToCourier()
        {
            try
            {
                // fetch all deliveries that are on pool.
                var adminPoolCity = User?.FindFirstValue("pool_city").ToString();
                var deliveriesOnPool = await _context.Delivery.Where(d => d.delivery_status == "on_destination_pool" && d.pool_receiver_city == adminPoolCity).ToListAsync();

                var prioDeliveries = deliveriesOnPool.Where(prio => prio.service_type == "PRIO");
                var regDeliveries = deliveriesOnPool.Where(prio => prio.service_type == "REG");
                var combinedPrioritizedDeliveries = prioDeliveries.Concat(regDeliveries).ToList();

                var availableCouriers = await _context.Courier.Where(d => d.courier_availability == true && d.pool_city == adminPoolCity).ToListAsync();

                List<Object> assignmentList = new List<Object> {};
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
                        var assigned_data = new
                        {
                            tracking_number = delivery.tracking_number,
                            courier_id = availableCouriers[i].courier_id,
                            courier_name = availableCouriers[i].courier_username,
                            pool_city = delivery.pool_receiver_city,
                        };
                        assignmentList.Add(assigned_data);
                    }
                    catch (Exception exc)
                    {
                        continue;
                    }
                }
                await _context.SaveChangesAsync();

                return Ok(assignmentList);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.Message);
            }
        }

        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH
        ///     {
        ///       "tracking_number": "PRIO01122300004",
        ///       "message_text": "Package is received by satpam.",
        ///       "timestamp": "2023-12-02T15:14:25.812764+07:00"
        ///     }
        /// </remarks>
        /// <param name="arrived"></param>
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
                    return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.Message);;
                }
            }

        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH
        ///     {
        ///       "tracking_number": "PRIO01122300004",
        ///       "message_text": "Package is rejected. \"gagal uy\"",
        ///       "timestamp": "2023-12-02T15:14:25.812764+07:00"
        ///     }
        /// </remarks>
        /// <param name="failedDelivery"></param>
        [HttpPatch("/failedDelivery"), Authorize]
        public async Task<IActionResult> FailedDelivery(String tracking_number, String courier_message)
        {
            try
            {
                var admin_pool_city = User?.FindFirstValue("pool_city").ToString();
                var delivery = await _context.Delivery.Where(d => d.tracking_number == tracking_number).FirstOrDefaultAsync();
                if (admin_pool_city != delivery.pool_receiver_city)
                    return BadRequest($"You are not the admin of {delivery.pool_receiver_city}! Please use the correct credentials to update package status!");
                if (delivery.delivery_status == "otw_receiver_address")
                {
                    delivery.delivery_status = "delivery_failed";
                    delivery.fail_message = courier_message;
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
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.Message);;
            }
        }

        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH
        ///     {
        ///       "tracking_number": "REG2023120200001",
        ///       "message_text": "Package has been returned to Tangerang pool.",
        ///       "timestamp": "2023-12-03T01:30:14.536941+07:00"
        ///     }
        /// </remarks>
        /// <param name="returnedToPool"></param>
        [HttpPatch("/returnedToPool")]

        public async Task<IActionResult> ReturnedToPool(String tracking_number)
        {
            try
            {
                var delivery = await _context.Delivery.Include(d => d.Courier).Where(d => d.tracking_number == tracking_number).FirstOrDefaultAsync();
                if (delivery.delivery_status == "delivery_failed")
                {
                    delivery.delivery_status = "returned_to_pool";
                    delivery.Courier.courier_availability = true;
                    delivery.returned_status = true;
                    var newMessage = new Message()
                    {
                        message_text = $"Package has been returned to {delivery.pool_receiver_city} pool.",
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
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.InnerException.Message); ;
            }
        }

    }
}





    