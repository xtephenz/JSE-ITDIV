using System;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models
{
	public class Admin
	{
        [Key]
        public string admin_id { get; set; }

        [MaxLength(255)]
        public string admin_username { get; set; }


        [MaxLength(255)]
        public string admin_password { get; set; }
    }
}

