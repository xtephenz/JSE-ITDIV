using System;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models
{
	public class Courier
	{
		[Key]
		public Guid courier_id { get; set; }

        [MaxLength(255)]
        public string courier_name { get; set; }


        [StringLength(13, MinimumLength = 10, ErrorMessage = "Must be between 10 and 13 characters long.")]
        public string courier_phone { get; set; }

    }
}

