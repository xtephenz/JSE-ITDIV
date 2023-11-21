
namespace JSE.Models.Results
{
    public class GetDeliveryResult
    {
        public string tracking_number { get; set; }
                
        public string service_type { get; set; }
                
        public DateTime sending_date { get; set; }
             
        public DateTime arrival_date { get; set; }
               
        public int package_weight { get; set; }
              
        public int delivery_price { get; set; }

        public string delivery_status { get; set; }

        public string sender_name { get; set; }

        public string sender_phone { get; set; }

        public string sender_address { get; set; }

        public string receiver_name { get; set; }

        public string receiver_address { get; set; }

        public string receiver_phone { get; set; }

        public string pool_id { get; set; }

        public string courier_id { get; set; }
    }
}