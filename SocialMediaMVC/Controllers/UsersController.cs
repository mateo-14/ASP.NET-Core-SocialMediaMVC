using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaMVC.Services.UsersService;
using System.Security.Claims;

namespace SocialMediaMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("users/{userId}/followers")]
        [Authorize]
        public async Task<IActionResult> AddFollow(string userId)
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (clientId is null)
            {
                return Unauthorized();
            }

            if (userId == clientId)
            {
                return BadRequest("You cannot follow yourself");
            }

            var result = await _usersService.AddFollow(userId, clientId);
            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while following the user");
            }
            return NoContent();
        }

        [HttpDelete("users/{userId}/followers")]
        [Authorize]
        public async Task<IActionResult> RemoveFollow(string userId)
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (clientId is null)
            {
                return Unauthorized();
            }

            if (userId == clientId)
            {
                return BadRequest("You cannot unfollow yourself");
            }

            var result = await _usersService.RemoveFollow(userId, clientId);
            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while unfollowing the user");
            }

            return NoContent();
        }
    }
}
