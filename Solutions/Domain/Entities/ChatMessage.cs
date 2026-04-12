using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("ChatMessage")]
    public class ChatMessage
    {
        [Key]
        public int ChatMessageId { get; set; }

        public int ChatId { get; set; }

        public int SenderId { get; set; }

        [Column("ChatMessage")]
        public string Content { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        [Column("SentAt")]
        public DateTime Timestamp { get; set; }

        public DateTime? ReadAt { get; set; }
    }
}
