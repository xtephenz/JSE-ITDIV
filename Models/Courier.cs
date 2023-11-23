using System;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models
{
	public class Courier
	{
		[Key]
		public Guid courier_id { get; set; }

        [MaxLength(255)]
        public string courier_username { get; set; }


        [Phone]
        public string courier_phone { get; set; }

        [Required]
        public string courier_password { get; set; }


        public bool courier_availability { get; set; } = true;

    }
}

