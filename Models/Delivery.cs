using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSE.Models
{
	public class Delivery
	{
        [Key]
        public string tracking_number { get; set; }

        [Required]
        public DateTime sending_date { get; set; }

        [MaxLength(255)]
        public string service_type { get; set; }

        [Required]
        public int package_weight { get; set; }

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

        [ForeignKey("Courier")]
        public Guid? courier_id { get; set; }

        public Courier Courier { get; set; }

        [ForeignKey("Admin")]
        public Guid? admin_id { get; set; }

        public Courier Admin { get; set; }

        [MaxLength(255)]
        public string fail_message { get; set; }

        [Required]
        public DateTime arrival_date { get; set; }

        public byte bukti_gambar { get; set; }

        public bool returned_status { get; set; }

        [ForeignKey("SenderPool")]
        public string pool_sender_city { get; set; }

        public PoolBranch SenderPool { get; set; }

        [ForeignKey("ReceiverPool")]
        public string pool_receiver_city { get; set; }

        public PoolBranch ReceiverPool { get; set; }

    }
}

