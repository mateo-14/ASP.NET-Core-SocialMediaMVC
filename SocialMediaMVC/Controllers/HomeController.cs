using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaMVC.Data;
using SocialMediaMVC.Models;
using SocialMediaMVC.Services.PostsService;
using SocialMediaMVC.Services.UsersService;
using SocialMediaMVC.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace SocialMediaMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsersService _usersService;
        private readonly IPostsService _postsService;

        public HomeController(ILogger<HomeController> logger, IUsersService usersService, IPostsService postsService)
        {
            _logger = logger;
            _usersService = usersService;
            _postsService = postsService;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetUser(string userName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _usersService.GetUserByUserName(userName, userId);
            if (user is null)
            {
                return NotFound();
            }

            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
