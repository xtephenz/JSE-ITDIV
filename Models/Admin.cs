using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSE.Models
{
	public class Admin
	{
        [Key]
        public Guid admin_id { get; set; }

        [MaxLength(255)]
        public string admin_username { get; set; }

        [Required]
        [ForeignKey("PoolBranch")]
        public string pool_city { get; set; }

        public PoolBranch PoolBranch { get; set; }

        [MaxLength(255)]
        public string admin_password { get; set; }
    }
}

