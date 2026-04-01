using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ClaimResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int? RequestId { get; set; }
    }
}
