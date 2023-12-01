using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JSE.Models
{
    public class Message
    {
        [Key]
        public Guid id { get; set; }

        [ForeignKey("Delivery")]
        [Required]
        public string tracking_number { get; set; }

        public Delivery Delivery { get; set; }

        [Required]
        public string? message_text {  get; set; }

        [Required]
        public DateTime? timestamp { get; set; }

    }
}
