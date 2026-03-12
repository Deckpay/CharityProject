using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ChatMessageResponseDto
    {
        public int Id { get; set; }
        public int SenderId{ get; set; }
        public string Content { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    }
}
