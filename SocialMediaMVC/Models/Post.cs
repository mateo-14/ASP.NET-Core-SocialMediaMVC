namespace SocialMediaMVC.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public List<string> Images { get; set; }
        public string AuthorId { get; set; }
        public User Author { get; set; }
        public ICollection<User> LikedBy { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
