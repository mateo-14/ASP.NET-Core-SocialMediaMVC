using SocialMediaMVC.Models;

namespace SocialMediaMVC.Services.UsersService
{
    public interface IUsersService
    {
        /// <summary>
        /// This method returns a user by their username with last 5 posts.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="clientId">An optional param. Used to determine if the user is followed by the client, if the client is following the user and if the posts are liked by the client.</param>
        /// <returns></returns>
        public Task<UserDto?> GetUserByUserName(string userName, string? clientId = null);
    }
}
