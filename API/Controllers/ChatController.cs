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

        /// <summary>
        /// Új chat üzenetet küld.
        /// </summary>
        /// <param name="dto">Az elküldendő üzenet adatai.</param>
        /// <returns>200 OK, ha az üzenet küldése sikeres.</returns>
        [HttpPost("send")]
        public async Task<IActionResult> Send(ChatMessageRequestDto dto)
        {
            await _chatService.SendMessageAsync(dto);
            return Ok();
        }

        /// <summary>
        /// Lekéri egy adott igényléshez tartozó chatelőzményeket.
        /// </summary>
        /// <param name="requestId">Az igénylés azonosítója.</param>
        /// <returns>200 OK a chatelőzményekkel, vagy 401 Unauthorized, ha a felhasználó nem azonosítható.</returns>
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

        /// <summary>
        /// Lekéri egy adott chathez tartozó alapinformációkat.
        /// </summary>
        /// <param name="requestId">Az igénylés azonosítója.</param>
        /// <returns>200 OK a chat információival.</returns>
        [HttpGet("info/{requestId}")]
        public async Task<IActionResult> GetChatInfo(int requestId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var info = await _chatService.GetChatInfoAsync(requestId, userId);
            return Ok(info);
        }

        /// <summary>
        /// Lekéri a bejelentkezett felhasználó összes olvasatlan üzenetének számát.
        /// </summary>
        /// <returns>200 OK az olvasatlan üzenetek számával.</returns>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var count = await _chatService.GetTotalUnreadCountAsync(userId);
            return Ok(count);
        }

        /// <summary>
        /// Az adott igényléshez tartozó összes üzenetet olvasottnak jelöli.
        /// </summary>
        /// <param name="requestId">Az igénylés azonosítója.</param>
        /// <returns>200 OK, ha a művelet sikeres.</returns>
        [HttpPost("mark-read/{requestId}")]
        public async Task<IActionResult> MarkAsAllRead(int requestId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _chatService.MarkAsAllReadAsync(requestId,userId);
            return Ok();
        }
    }
}
