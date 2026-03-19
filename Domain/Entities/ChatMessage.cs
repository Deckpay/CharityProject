using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("ChatMessage")] // EZ KELL: Megmondja az EF-nek, hogy a tábla NEVE egyes számban van
    public class ChatMessage
    {
        [Key]
        [Column("ChatMessageId")] // Stimmel az SQL-lel
        public int Id { get; set; }

        [Column("ChatId")] // Stimmel az SQL-lel
        public int RequestId { get; set; }

        [Column("ChatMessage")] // Stimmel az SQL-lel
        public string Content { get; set; } = string.Empty;

        [Column("SentAt")] // Biztosítsuk be, hogy az SQL 'SentAt' oszlopát használja
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Column("SenderId")] // Stimmel az SQL-lel
        public int SenderId { get; set; }

        public User Sender { get; set; } = null!;

        [Column("IsRead")] // Stimmel az SQL-lel
        public bool IsRead { get; set; } = false;

        // Ha van ReadAt oszlopod is az SQL-ben, érdemes felvenni:
        // public DateTime? ReadAt { get; set; } 
    }
}
