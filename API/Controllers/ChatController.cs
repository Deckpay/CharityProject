using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService) => _chatService = chatService;

        [HttpPost("send")]
        public async Task<IActionResult> Send(ChatMessageRequestDto dto)
        {
            await _chatService.SendMessageAsync(dto);
            return Ok();
        }

        [HttpGet("history/{requestId}")]
        public async Task<IActionResult> GetHistory(int requestId)
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

           
            if (string.IsNullOrEmpty(claimValue))
            {
                return Unauthorized("Nem található a felhasználó");
            }

            var userId = int.Parse(claimValue);
            var history = await _chatService.GetChatHistoryAsync(requestId, userId);

            return Ok(history);
        }
    }
}