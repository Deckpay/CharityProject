namespace Application.DTOs
{
    public class ChatMessageRequestDto
    {
        public int RequestId { get; set; } // itt kutjuk ossze a chatet matchel
        
        public string Content { get; set; } = string.Empty;
    }
}
