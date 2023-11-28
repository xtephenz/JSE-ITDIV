using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JSE.Models
{
	public class PoolBranch
	{
        [Key]
        public string pool_name { get; set; }

        [StringLength(13, MinimumLength = 10, ErrorMessage = "Must be between 10 and 13 characters long.")]
        public string pool_phone { get; set; }

        [JsonIgnore]
        public ICollection<Delivery> SendingDeliveries { get; set; }
        [JsonIgnore]
        public ICollection<Delivery> ReceivingDeliveries { get; set; }
    }
}

