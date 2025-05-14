using APIArkanoid.Database;
using APIArkanoid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIArkanoid.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("skins")]
        public async Task<IActionResult> GetAvailableSkins()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var allSkins = await _context.BallSkins.ToListAsync();
            var ownedSkins = await _context.UserBallSkins
                .Where(ubs => ubs.UserId == userId)
                .Select(ubs => ubs.BallSkinId)
                .ToListAsync();

            var result = allSkins.Select(skin => new
            {
                skin.Id,
                skin.Name,
                skin.Price,
                skin.TexturePath,
                IsOwned = ownedSkins.Contains(skin.Id) || skin.IsDefault,
                IsEquipped = skin.IsDefault || _context.UserBallSkins
                    .Any(ubs => ubs.UserId == userId &&
                               ubs.BallSkinId == skin.Id &&
                               ubs.IsEquipped)
            });

            return Ok(result);
        }

        [Authorize]
        [HttpPost("buy-skin/{skinId}")]
        public async Task<IActionResult> BuySkin(int skinId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FindAsync(userId);
            var skin = await _context.BallSkins.FindAsync(skinId);

            // Проверки
            if (skin == null) return BadRequest("Скин не найден");
            if (user.coins < skin.Price) return BadRequest("Недостаточно монет");

            // Списание монет и сохранение скина
            user.coins -= skin.Price;
            _context.UserBallSkins.Add(new UserBallSkin
            {
                UserId = userId,
                BallSkinId = skinId,
                IsEquipped = false
            });

            await _context.SaveChangesAsync();
            return Ok(new { NewBalance = user.coins });
        }

        [HttpPost("equip-skin/{skinId}")]
        public async Task<IActionResult> EquipSkin(int skinId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Check if user owns the skin or it's default
            var skin = await _context.BallSkins.FindAsync(skinId);
            if (skin == null)
                return BadRequest("Invalid skin");

            if (!skin.IsDefault && !await _context.UserBallSkins.AnyAsync(ubs => ubs.UserId == userId && ubs.BallSkinId == skinId))
                return BadRequest("Skin not owned");

            // Unequip all other skins
            var equippedSkins = await _context.UserBallSkins
                .Where(ubs => ubs.UserId == userId && ubs.IsEquipped)
                .ToListAsync();

            foreach (var equippedSkin in equippedSkins)
            {
                equippedSkin.IsEquipped = false;
            }

            // Equip the new skin (if not default)
            if (!skin.IsDefault)
            {
                var userSkin = await _context.UserBallSkins
                    .FirstOrDefaultAsync(ubs => ubs.UserId == userId && ubs.BallSkinId == skinId);

                userSkin.IsEquipped = true;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
