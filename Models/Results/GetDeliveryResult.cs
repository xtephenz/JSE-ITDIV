using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models.Results
{
    public class GetDeliveryResult
    {
        public string tracking_number { get; set; }

        // sender columns
        public DateTime sending_date { get; set; }

        public string sender_name { get; set; }

        public string sender_phone { get; set; }

        public string sender_address { get; set; }



        // recipient columns

        public string intended_receiver_name { get; set; }

        public string receiver_phone { get; set; }

        public string receiver_address { get; set; }

        // package descriptions
        public string service_type { get; set; }
        //public int package_weight { get; set; }
        public int delivery_price { get; set; }

        public string delivery_status { get; set; }

        public string? actual_receiver_name { get; set; }

        public Guid? courier_id { get; set; }
        public GetCourierResult Courier { get; set; }
        public DateTime? arrival_date { get; set; }

        public bool? returned_status { get; set; }

        public string? fail_message { get; set; }

        // Pool definitions
        public string pool_sender_city { get; set; }

        public GetPoolResult SenderPool { get; set; }

        public string pool_receiver_city { get; set; }

        public GetPoolResult ReceiverPool { get; set; }

        // messages
        public ICollection<GetMessageResult> Messages { get; set; }
    }
}
