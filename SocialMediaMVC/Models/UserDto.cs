namespace SocialMediaMVC.Models
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public int TotalFollowers { get; set; }
        public int TotalFollowing { get; set; }
        public bool IsFollowingClient { get; set; }
        public bool IsFollowedByClient { get; set; }
        public List<PostDto>? Posts { get; set; }
        public string? ProfileImage { get; set; }
    }
}
