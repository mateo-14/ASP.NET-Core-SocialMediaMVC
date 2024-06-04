using SocialMediaMVC.Models;

namespace SocialMediaMVC.ViewModels
{
    public class GetUserViewModel
    {
        public User User { get; set; }
        public List<Post> Posts { get; set; }
    }
}
