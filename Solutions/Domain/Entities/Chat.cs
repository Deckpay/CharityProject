using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Chat")]
    public class Chat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int ChatId { get; set; }
        public int ProductRequestId { get; set; }
        public int SenderId { get; set; }
        public int RequesterId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
