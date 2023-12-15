using Microsoft.Identity.Client;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSE.Models.Requests
{
	public class GetDeliveryListCourier
	{
		public string tracking_number { get; set; }

        public string delivery_status { get; set; }

        public string intended_receiver_name { get; set; }

        public string actual_receiver_name { get; set; }

        public string receiver_phone { get; set; }

        public string receiver_address { get; set; }

        public Guid courier_id { get; set; }

        public string fail_message { get; set; }

        public DateTime arrival_date { get; set; }

        //public byte bukti_gambar { get; set; }

        public string pool_receiver_city { get; set; }
    }

   
}

