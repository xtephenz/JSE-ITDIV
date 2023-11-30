using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSE.Models.Requests
{
	public class CourierRegisterRequest
	{
        public string courier_username { get; set; } = string.Empty;

        public string courier_password { get; set; } = string.Empty;

        public string courier_confirm_password { get; set; } = string.Empty;

        public string courier_phone { get; set; }
    }
}

