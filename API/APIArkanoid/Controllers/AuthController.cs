using APIArkanoid.Models;
using APIArkanoid.Services;
using Microsoft.AspNetCore.Mvc;

namespace APIArkanoid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Имя пользователя и пароль обязательны");
            }

            var token = await _authService.Authenticate(request.Username, request.Password);

            if (token == null)
                return Unauthorized();


            return Ok(new { Token = token });
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
