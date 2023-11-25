using System;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models.Requests
{
	public class CourierLoginRequest
	{
		
        [Required]
<<<<<<< Updated upstream
        public string admin_username{ get; set; } = string.Empty;
=======
        public string courier_username { get; set; } = string.Empty;
>>>>>>> Stashed changes

        [Required]
        public string courier_password { get; set; } = string.Empty;
        
	}
}

