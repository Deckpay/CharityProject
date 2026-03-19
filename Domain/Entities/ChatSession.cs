namespace Domain.Entities
{
    public class ChatSession
    {
        public Guid Id { get; set; } // 128 bit numerikus azonosito 
        public Guid RequestId { get; set; } // igenyles alapú
        public string DonorId { get; set; } = string.Empty;
        public string RecipientId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
