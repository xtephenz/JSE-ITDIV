using System;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models.Results
{
	public class GetCourierResult
	{
        public string courier_username { get; set; }
        public string courier_phone { get; set; }

        public bool courier_availability { get; set; }
    }
}

