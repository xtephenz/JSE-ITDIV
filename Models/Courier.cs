using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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

        
        [ForeignKey("PoolBranch")]
        [Required]
        public string pool_city { get; set; }
        public PoolBranch PoolBranch { get; set; }

        public bool courier_availability { get; set; } = true;
        [JsonIgnore]


        public List<Delivery> Deliveries { get; set; }
    }
}

