using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models.Requests
{
    public class CreateDelivery
    {
        public string tracking_number { get; set; }
        [Required]
        public DateTime sending_date { get; set; }
        public string sender_name { get; set; }

        [Phone]
        public string sender_phone { get; set; }

        [MaxLength(255)]
        public string sender_address { get; set; }

        // recipient columns

        public string intended_receiver_name { get; set; }

        [Phone]
        public string receiver_phone { get; set; }

        [MaxLength(255)]
        public string receiver_address { get; set; }

        // package descriptions
        [MaxLength(255)]
        public string service_type { get; set; }

        [Required]
        public int package_weight { get; set; }

        [Required]
        public int delivery_price { get; set; }

        [MaxLength(255)]
        public string delivery_status { get; set; }


        // post delivery
        public string pool_sender_city { get; set; }
        public string pool_receiver_city { get; set; }

    }
}
