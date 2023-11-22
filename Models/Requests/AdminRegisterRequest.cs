using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSE.Models.Requests
{
	public class AdminRegisterRequest
	{
        [Required]
        public string admin_username { get; set; } = string.Empty;

        [Required]
        [ForeignKey("PoolBranch")]
        public string pool_city { get; set; } = string.Empty;
        public string admin_password { get; set; } = string.Empty;
        [Required]

        public string admin_confirm_password { get; set;} = string.Empty;
	}
}

