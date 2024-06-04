using Microsoft.EntityFrameworkCore;
using SocialMediaMVC.Data;
using SocialMediaMVC.Models;
using System.Linq;

namespace SocialMediaMVC.Services.UsersService
{
    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersService> _logger;

        public UsersService(ApplicationDbContext context, ILogger<UsersService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserDto?> GetUserByUserName(string userName, string? clientId = null)
        {
            var user = await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    TotalFollowers = u.Followers.Count,
                    TotalFollowing = u.Following.Count,
                    IsFollowingClient = u.Following.Any(f => f.Id == clientId),
                    IsFollowedByClient = u.Followers.Any(f => f.Id == clientId),
                    ProfileImage = u.ProfileImage,
                    Posts = u.Posts.Select(p => new PostDto
                    {
                        Id = p.Id,
                        Content = p.Content,
                        CreatedAt = p.CreatedAt,
                        TotalLikes = p.LikedBy.Count,
                        Images = p.Images,
                        LikedByClient = p.LikedBy.Any(l => l.Id == clientId),
                        Author = new UserDto
                        {
                            Id = u.Id,
                            UserName = u.UserName,
                            Email = u.Email,
                            TotalFollowers = u.Followers.Count,
                            TotalFollowing = u.Following.Count,
                            ProfileImage = u.ProfileImage
                        }
                    }).OrderByDescending(p => p.CreatedAt).Take(5).ToList()

                }).FirstOrDefaultAsync(u => u.UserName == userName);

            return user;
        }

        public async Task<bool> AddFollow(string userId, string clientId)
        {
            var client = new User
            {
                Id = clientId
            };

            var user = new User
            {
                Id = userId,
                Followers = new List<User>()
            };

            _context.Attach(client);
            _context.Attach(user);
            user.Followers.Add(client);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a follow");
                return false;
            }
        }

        public async Task<bool> RemoveFollow(string userId, string clientId)
        {
            var client = new User
            {
                Id = clientId
            };

            var user = new User
            {
                Id = userId,
                Followers = new List<User>() { client }
            };

            _context.Attach(user);
            user.Followers.Remove(client);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing a follow");
                return false;
            }
        }
    }
}
