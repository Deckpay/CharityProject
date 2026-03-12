using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; } // elsodleges kulcs
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } 
        public int SenderId { get; set; }
        public User Sender { get; set; } = null!;
        public int ReceiverId { get; set; }
        public User Receiver { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        
        

    }
}
