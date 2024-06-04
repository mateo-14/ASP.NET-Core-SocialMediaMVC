namespace SocialMediaMVC.Models
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public List<string> Images { get; set; }
        public UserDto Author { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int TotalLikes { get; set; }
        public bool LikedByClient { get; set; }
    }
}
