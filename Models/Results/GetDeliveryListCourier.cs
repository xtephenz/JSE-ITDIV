using Microsoft.Identity.Client;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSE.Models.Requests
{
	public class GetDeliveryListCourier
	{
		public string tracking_number;

        public string delivery_status { get; set; }

        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
         ErrorMessage = "Characters are not allowed.")]
        public string actual_receiver_name { get; set; }

        [Phone]
        public string receiver_phone { get; set; }

        [MaxLength(255)]
        public string receiver_address { get; set; }

        public Guid courier_id { get; set; }

        [MaxLength(255)]
        public string fail_message { get; set; }

        [Required]
        public DateTime arrival_date { get; set; }

        //public byte bukti_gambar { get; set; }

        public string pool_name { get; set; }

        [StringLength(13, MinimumLength = 10, ErrorMessage = "Must be between 10 and 13 characters long.")]
        public string pool_phone { get; set; }
    }

   
}

