using System;
using System.ComponentModel.DataAnnotations;
namespace JSE.Models
{
    public class GetSuccessDelivery {
        [Required]
        public string tracking_number { get; set; }

        [Required]
        public string receiver_name { get; set; }

    }
}