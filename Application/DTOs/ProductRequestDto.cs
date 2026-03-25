using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProductRequestDto
    {
        public int ProductRequestId { get; set; }
        public int ProductId { get; set; }
        public int RequesterId { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
