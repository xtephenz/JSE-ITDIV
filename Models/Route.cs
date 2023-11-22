using System;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models
{
	public class Route
	{
        [Key]
        [Required]
        
        public string route_id { get; set; }

        [MaxLength(255)]
        public string pool_sender_city { get; set; }

        [Phone]
        public string pool_phone { get; set; }
    }
}

