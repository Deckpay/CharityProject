using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("ChatMessage")] // EZ KELL: Megmondja az EF-nek, hogy a tábla NEVE egyes számban van
    public class ChatMessage
    {
        [Key]

        public int ChatMessageId { get; set; }

        public int ChatId { get; set; }

        public int SenderId { get; set; }

        // Megmondjuk az EF-nek, hogy a C#-os 'Content' az SQL-ben 'ChatMessage' néven fut
        [Column("ChatMessage")]
        public string Content { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        // Megmondjuk az EF-nek, hogy a 'Timestamp' valójában a 'SentAt' oszlop
        [Column("SentAt")]
        public DateTime Timestamp { get; set; }

        // Mivel az SQL-ben van egy ReadAt oszlopod is, ezt is felvesszük (nullable-ként, mert nem biztos, hogy rögtön elolvassák)
        public DateTime? ReadAt { get; set; }
    }
}
