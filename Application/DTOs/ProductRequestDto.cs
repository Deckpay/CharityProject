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
        public int RequestStatus { get; set; }
        public bool IsActive { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
