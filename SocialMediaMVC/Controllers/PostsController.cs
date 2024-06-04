using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaMVC.Data;
using SocialMediaMVC.Models;
using SocialMediaMVC.Services.PostsService;
using SocialMediaMVC.ViewModels;
using System.Security.Claims;

namespace SocialMediaMVC.Controllers
{
    public class PostsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IPostsService _postsService;

        public PostsController(ApplicationDbContext context, UserManager<User> userManager, IPostsService postsService)
        {
            _userManager = userManager;
            _postsService = postsService;
        }

        public async Task<IActionResult> Index(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = await _postsService.GetPost(id, userId);
            return View(post);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var post = await _postsService.CreatePost(model.Content ?? string.Empty, model.Images, userId);
            if (post is null)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the post");
                return View(model);
            }

            return RedirectToAction("Index", new { id = post.Id });
        }

        [HttpPost("posts/{id}/likes")]
        [Authorize]
        public async Task<IActionResult> AddLike(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }

            var added = await _postsService.AddLike(id, userId);
            if (!added)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the like");
            }

            return NoContent();
        }

        [HttpDelete("posts/{id}/likes")]
        [Authorize]
        public async Task<IActionResult> RemoveLike(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized();
            }

            var removed = await _postsService.RemoveLike(id, userId);
            if (!removed)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while removing the like");
            }

            return NoContent();
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts(int skip = 0, int take = 10, string? authorId = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var posts = await _postsService.GetPosts(skip, take, authorId, userId);
            return Ok(posts);
        }
    }
}
