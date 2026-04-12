namespace Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
