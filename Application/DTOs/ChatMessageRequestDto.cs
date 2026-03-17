using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ChatMessageRequestDto
    {
        public int RequestId { get; set; } // itt kutjuk ossze a chatet matchel
        
        public string Content { get; set; } = string.Empty;
    }
}
