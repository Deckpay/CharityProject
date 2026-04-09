namespace Application.DTOs
{
    public class ChatMessageRequestDto
    {
        public int RequestId { get; set; }
        
        public string Content { get; set; } = string.Empty;
    }
}
