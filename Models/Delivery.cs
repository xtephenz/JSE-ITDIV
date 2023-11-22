using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSE.Models
{
	public class Delivery
	{
        [Key]
        public string tracking_number { get; set; }

        [MaxLength(255)]
        public string service_type { get; set; }

        [Required]
        public DateTime sending_date { get; set; }

        [Required]
        public DateTime arrival_date { get; set; }

        [Required]
        public int package_weight { get; set; }

        [Required]
        public int delivery_price { get; set; }

        [MaxLength(255)]
        public string delivery_status { get; set; }

        [MaxLength(255)]
        public string sender_name { get; set; }

        [StringLength(13, MinimumLength = 10, ErrorMessage = "Must be between 10 and 13 characters long.")]
        public string sender_phone { get; set; }

        [MaxLength(255)]
        public string sender_address { get; set; }

        [MaxLength(255)]
        public string receiver_name { get; set; }

        [MaxLength(255)]
        public string receiver_address { get; set; }

        [StringLength(13, MinimumLength = 10, ErrorMessage = "Must be between 10 and 13 characters long.")]
        public string receiver_phone { get; set; }







        // testing


        [ForeignKey("SenderPool")]
        public string sender_city { get; set; }


        [ForeignKey("ReceiverPool")]
        public string receiver_city { get; set; }
        public PoolBranch SenderPool { get; set; }
        public PoolBranch ReceiverPool { get; set; }



        [ForeignKey("Courier")]
        public Guid? courier_id { get; set; }
        public Courier Courier { get; set; }


    }
}

