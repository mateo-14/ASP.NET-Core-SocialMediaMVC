using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SocialMediaMVC.Models;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    public ICollection<Post> Posts { get; set; }
    public ICollection<Post> LikedPosts { get; set; }
    public ICollection<User> Followers { get; set; }
    public ICollection<User> Following { get; set; }
    public string? ProfileImage { get; set; }
}

