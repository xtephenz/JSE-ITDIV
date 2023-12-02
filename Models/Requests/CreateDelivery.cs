using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models.Requests
{
    public class CreateDelivery
    {
        public string sender_name { get; set; }

        public string sender_phone { get; set; }

        public string sender_address { get; set; }

        // recipient columns

        public string intended_receiver_name { get; set; }

        public string receiver_phone { get; set; }

        public string receiver_address { get; set; }

        // package descriptions
        public string service_type { get; set; }

        public int package_weight { get; set; }

        public int delivery_price { get; set; }


        // post delivery
        public string pool_sender_city { get; set; }
        public string pool_receiver_city { get; set; }

    }
}
