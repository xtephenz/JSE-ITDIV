using System;
using System.ComponentModel.DataAnnotations;
namespace JSE.Models
{
    public class GetCancelRequest
    {
        [Required]
        public string reason { get; set; }

        [Required]
        public string tracking_number { get; set; }

    }
}