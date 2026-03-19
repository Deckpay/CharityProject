namespace Application.DTOs
{
    public class ChatMessageResponseDto
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int SenderId{ get; set; }
        public string Content { get; set; } = string.Empty;        
        public string SenderName { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
