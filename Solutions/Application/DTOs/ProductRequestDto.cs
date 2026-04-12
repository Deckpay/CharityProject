using Domain.Enums;

namespace Application.DTOs
{
    public class ProductRequestDto
    {
        public int ProductRequestId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int RequesterId { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
