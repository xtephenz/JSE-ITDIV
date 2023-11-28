using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JSE.Models.Results
{
    public class GetMessageResult
    {
        public string tracking_number { get; set; }
        public string? message_text { get; set; }
        public DateTime? timestamp { get; set; }

    }
}
