using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // kontroller bekéri az autservice-t
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // rész automatikusan behelyettesíti az osztály nevét, levágva a "Controller" szót
        [HttpPost("register")] // Ez lesz a végpont: POST api/Auth/register
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // 1. A kontroller csak továbbpasszolja a labdát a servicenek
            var success = await _authService.RegisterAsync(dto);

            // 2. Visszaszól a Web-nek, hogy sikerűlt e
            if (success) return Ok();
            return BadRequest("A regisztráció sikertelen");
        }

        [HttpGet("counties")] // GET api/Auth/counties
        public async Task<IActionResult> GetCounties()
        {
            var counties = await _authService.GetCountiesAsync();
            return Ok(counties);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var userLogin = await _authService.LoginAsync(loginDto.EmailOrUserName, loginDto.Password);

            if (userLogin != null) return Ok(userLogin);
            return Unauthorized("Hibás adatok");
        }

    }
}
