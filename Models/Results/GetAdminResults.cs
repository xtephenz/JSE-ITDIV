using System;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models.Results
{
	public class GetAdminResults
	{
        [Key]
        public string admin_id { get; set; }

        [MaxLength(255)]
        public string admin_username { get; set; }


        [MaxLength(255)]
        public string admin_password { get; set; }
    }
}

