using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    [Table("ProductRequests")]
    public class ProductRequest
    {
        [Key]
        public int ProductRequestId { get; set; }
        public int ProductId { get; set; }
        public int RequesterId { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
