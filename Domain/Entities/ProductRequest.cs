using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("ProductRequests")] // megmondja EF-nek hogy egyes számban van a tábla neve
    public class ProductRequest
    {
        [Key]
        public int ProductRequestId { get; set; }
        public int ProductId { get; set; }
        public int RequesterId { get; set; }
        public int RequestStatus { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
