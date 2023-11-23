using System;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models
{
	public class PoolBranch
	{
        [Key]
        public string pool_name { get; set; }

        [StringLength(13, MinimumLength = 10, ErrorMessage = "Must be between 10 and 13 characters long.")]
        public string pool_phone { get; set; }

        public ICollection<Delivery> SendingDeliveries { get; set; }
        
        public ICollection<Delivery> ReceivingDeliveries { get; set; }
    }
}

