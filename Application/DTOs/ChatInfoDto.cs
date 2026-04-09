namespace Application.DTOs
{
    public class ChatInfoDto
    {
        public int SenderId { get; set; }
        public string OtherPartyName { get; set; } = string.Empty;
        public string? ProductName { get; set; }
    }
}
