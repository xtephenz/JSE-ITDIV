using System;
using System.ComponentModel.DataAnnotations;
namespace JSE.Models
{
    public class GetFailedDelivery {
        [Required]
        public string tracking_number { get; set; }

        [Required]
        public string reason { get; set; }

    }
}