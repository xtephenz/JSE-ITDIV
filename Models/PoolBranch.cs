using System;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models
{
	public class PoolBranch
	{
        [Key]
        public string pool_id { get; set; }

        [MaxLength(255)]
        public string pool_address { get; set; }

        [StringLength(13, MinimumLength = 10, ErrorMessage = "Must be between 10 and 13 characters long.")]
        public string pool_phone { get; set; }
    }
}

