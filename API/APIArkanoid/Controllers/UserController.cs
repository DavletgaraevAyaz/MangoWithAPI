using APIArkanoid.Database;
using APIArkanoid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace APIArkanoid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("coins/{id}")]
        public async Task<IActionResult> GetCoins(int id)
        {
            try
            {
            // Ищем количество монет пользователя в базе данных
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new { u.coins })
                .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                Console.WriteLine($"User coin from token:{user.coins}");
                // Возвращаем количество монет
                return new OkObjectResult(new { сoins = user.coins });
            }
            catch (Exception ex)
            {
                // Логируем ошибку (можно использовать ILogger для sлогирования)
                Console.Error.WriteLine($"Error fetching coins: {ex.Message}");
                return StatusCode(500, "Internal server error occurred while fetching coins.");
            }
        }

        [HttpPost("add-coins/{id}")]
        public async Task<IActionResult> AddCoins(int id,[FromBody] AddCoinsRequest request)
        {
            try
            {
                var userCur = await _context.Users
               .Where(u => u.Id == id)
               .FirstOrDefaultAsync();

                if (userCur == null)
                {
                    return NotFound("User not found.");
                }
                userCur.coins += request.Amount;
                await _context.SaveChangesAsync();

                return Ok(new { coins = userCur.coins });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error adding coins: {ex.Message}");
                return StatusCode(500, "Internal server error occurred while adding coins.");
            }
            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //var user = await _context.Users.FindAsync(userId);

            
        }

        [HttpGet("all-coins")]
        public async Task<IActionResult> GetAllUsersCoins()
        {
            try
            {
                // Получаем всех пользователей и их количество монет
                var users = await _context.Users
                    .Select(u => new { u.Username, u.coins }) // Предполагается, что у вас есть поле Username
                    .ToListAsync();

                if (users == null || !users.Any())
                {
                    return NotFound("No users found.");
                }

                // Возвращаем список пользователей с их монетами
                return Ok(new { users });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching all users' coins: {ex.Message}");
                return StatusCode(500, "Internal server error occurred while fetching users' coins.");
            }
        }


        public class AddCoinsRequest
        {
            public int Amount { get; set; }
        }
    }
}
