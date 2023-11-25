using System;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models.Requests
{
	public class CourierFailedRequest
	{
        [MaxLength(255)]
        public string? fail_message { get; set; }

        public string tracking_number { get; set; }
    }
}

