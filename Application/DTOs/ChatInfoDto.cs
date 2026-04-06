using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ChatInfoDto
    {
        public int SenderId { get; set; }
        public string OtherPartyName { get; set; } = string.Empty;
        public string? ProductName { get; set; }
    }
}
