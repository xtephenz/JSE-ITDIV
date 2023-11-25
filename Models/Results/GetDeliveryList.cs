using Microsoft.Identity.Client;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSE.Models.Requests
{
    //muncul ga?
	public class GetDeliveryListByCourier
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
        public string courier_username { get; set; }

        [MaxLength(255)]
        public string fail_message { get; set; }

        [Required]
        public DateTime arrival_date { get; set; }

        public byte bukti_gambar { get; set; }

        public string pool_name { get; set; }

        [StringLength(13, MinimumLength = 10, ErrorMessage = "Must be between 10 and 13 characters long.")]
        public string pool_phone { get; set; }
    }

    public class GetDeliveryListByAdmin
    {
        public string tracking_number { get; set; }

        [Required]
        public DateTime sending_date { get; set; }

        [MaxLength(255)]
        public string service_type { get; set; }

        [Required]
        public int delivery_price { get; set; }

        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
         ErrorMessage = "Characters are not allowed.")]
        public string intended_receiver_name { get; set; }

        [MaxLength(255)]
        public string delivery_status { get; set; }

        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
         ErrorMessage = "Characters are not allowed.")]
        public string sender_name { get; set; }

        [Phone]
        public string sender_phone { get; set; }

        [MaxLength(255)]
        public string sender_address { get; set; }

        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
         ErrorMessage = "Characters are not allowed.")]
        public string actual_receiver_name { get; set; }

        [Phone]
        public string receiver_phone { get; set; }

        [MaxLength(255)]
        public string receiver_address { get; set; }

        [MaxLength(255)]
        public string fail_message { get; set; }

        [Required]
        public DateTime arrival_date { get; set; }

        public byte bukti_gambar { get; set; }

        public bool returned_status { get; set; }

        [MaxLength(255)]
        public string courier_username { get; set; }

        [Phone]
        public string courier_phone { get; set; }
    }
}

