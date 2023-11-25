using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        [Column(TypeName = "varchar(255)")]
        [Required]
        public string courier_password { get; set; }

        public bool courier_availability { get; set; } = true;

        public List<Delivery> Deliveries { get; set; } = new List<Delivery>();
    }
}

