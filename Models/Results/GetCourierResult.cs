using System;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models.Results
{
	public class GetCourierResult
	{
        public string courier_id { get; set; }

        public string courier_name { get; set; }

        public string courier_phone { get; set; }
    }
}

