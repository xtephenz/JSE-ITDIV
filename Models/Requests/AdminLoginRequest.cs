using System;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models.Requests
{
	public class AdminLoginRequest
	{
        [Required]
        public string admin_username { get; set; } = string.Empty;

        [Required]
        public string admin_password { get; set; } = string.Empty;
	}
}

