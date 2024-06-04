using SocialMediaMVC.Models;
using System.Data.SqlTypes;

namespace SocialMediaMVC.Services.PostsService
{
    public interface IPostsService
    {
        /// <summary>
        /// Create a post with content and images
        /// </summary>
        /// <param name="content"></param>
        /// <param name="images"></param>
        /// <param name="authorId"></param>
        /// <returns>Created post or null if an error occurred</returns>
        public Task<PostDto?> CreatePost(string content, List<IFormFile> images, string authorId);
        /// <summary>
        /// Get a post by its id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientId">An optional param. Used to determine if the post is liked by the client, if the author is followed by the client and if the client is following the author.</param>
        /// <returns></returns>
        public Task<PostDto?> GetPost(int id, string? clientId = null);

        /// <summary>
        /// Get posts.
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="authorId">Used to filter posts by author</param>
        /// <param name="clientId">An optional param. Used to determine if the posts are liked by the client, if the author is followed by the client and if the client is following the author.</param>
        /// <returns></returns>
        public Task<List<PostDto>> GetPosts(int skip, int take, string? authorId = null, string? clientId = null);

        /// <summary>
        /// Add a like to a post
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="clientId"> Used to determine the client who liked the post</param>
        /// <returns>
        /// Returns true if the like was added successfully or false if something went wrong
        /// </returns>
        public Task<bool> AddLike(int postId, string clientId);

        /// <summary>
        /// Remove a like from a post
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="clientId"> Used to determine the client who unliked the post</param>
        /// <returns>
        /// Returns true if the like was removed successfully or false if something went wrong
        /// </returns>
        public Task<bool> RemoveLike(int postId, string clientId);
    }
}
