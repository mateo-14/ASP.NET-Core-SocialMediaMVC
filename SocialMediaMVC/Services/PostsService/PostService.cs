using Microsoft.EntityFrameworkCore;
using SocialMediaMVC.Data;
using SocialMediaMVC.Models;
using SocialMediaMVC.Services.FileUploadService;
using System.Linq.Expressions;

namespace SocialMediaMVC.Services.PostsService
{
    public class PostService : IPostsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUpload;
        private readonly ILogger<PostService> _logger;

        public PostService(ApplicationDbContext context, IFileUploadService fileUpload, ILogger<PostService> logger)
        {
            _context = context;
            _fileUpload = fileUpload;
            _logger = logger;
        }

        public async Task<PostDto?> CreatePost(string content, List<IFormFile> images, string authorId = null)
        {
            var author = await _context.Users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                TotalFollowers = u.Followers.Count,
                TotalFollowing = u.Following.Count
            }).FirstOrDefaultAsync(u => u.Id == authorId);

            if (author == null)
            {
                return null;
            }

            var post = new Post
            {
                Content = content,
                AuthorId = authorId,
                Images = new List<string>()
            };

            try
            {
                foreach (var image in images)
                {
                    var filePath = await _fileUpload.UploadFile(image);
                    post.Images.Add(filePath);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while uploading images for post {PostId}", post.Id);
                // Delete all uploaded images if an error occurs
                foreach (var image in post.Images)
                {
                    _fileUpload.DeleteFile(image);
                }

                return null;
            }

            _context.Posts.Add(post);
            try
            {
                await _context.SaveChangesAsync();

                return new PostDto
                {
                    Id = post.Id,
                    Content = post.Content,
                    Images = post.Images,
                    CreatedAt = post.CreatedAt,
                    Author = author,
                    TotalLikes = 0
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while saving post {PostId}", post.Id);
                // Delete all uploaded images if an error occurs
                foreach (var image in post.Images)
                {
                    _fileUpload.DeleteFile(image);
                }

                return null;
            }
        }

        public async Task<PostDto?> GetPost(int id, string? clientId = null)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .Select(CreatePostToPostDtoExpression(clientId))
                .FirstOrDefaultAsync(p => p.Id == id);

            return post;
        }

        public async Task<List<PostDto>> GetPosts(int skip, int take, string? authorId = null, string? clientId = null)
        {
            var query = _context.Posts
                .Include(p => p.Author)
                .OrderByDescending(p => p.CreatedAt)
                .Select(CreatePostToPostDtoExpression(clientId)).Skip(skip).Take(take);


            if (authorId is not null)
            {
                query = query.Where(p => p.Author.Id == authorId);
            }

            var posts = await query.ToListAsync();
            return posts;
        }

        public async Task<bool> AddLike(int postId, string clientId)
        {
            // Add like directly without checking if the post or user exists. This is to avoid unnecessary database calls, the 99% of the time the post and user will exist.
            var post = new Post { Id = postId, LikedBy = new List<User>() };
            var user = new User { Id = clientId };

            _context.Posts.Attach(post);
            _context.Users.Attach(user);
            post.LikedBy.Add(user);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while adding like for post {PostId}", post.Id);
                return false;
            }
        }

        public async Task<bool> RemoveLike(int postId, string clientId)
        {
            // Remove like directly without checking if the post or user exists. This is to avoid unnecessary database calls, the 99% of the time the post and user will exist.
            var user = new User { Id = clientId };
            var post = new Post { Id = postId, LikedBy = new List<User>() { user } };
            _context.Posts.Attach(post);
            post.LikedBy.Remove(user);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while removing like for post {PostId}", post.Id);
                return false;
            }
        }

        private Expression<Func<Post, PostDto>> CreatePostToPostDtoExpression(string? clientId) => p => new PostDto
        {
            Id = p.Id,
            Content = p.Content,
            CreatedAt = p.CreatedAt,
            TotalLikes = p.LikedBy.Count,
            Images = p.Images,
            LikedByClient = p.LikedBy.Any(u => u.Id == clientId),
            Author = new UserDto
            {
                Id = p.Author.Id,
                UserName = p.Author.UserName,
                Email = p.Author.Email,
                TotalFollowers = p.Author.Followers.Count,
                TotalFollowing = p.Author.Following.Count,
                IsFollowedByClient = p.Author.Followers.Any(f => f.Id == clientId),
                IsFollowingClient = p.Author.Following.Any(f => f.Id == clientId),
                ProfileImage = p.Author.ProfileImage
            },
        };
    }
}
