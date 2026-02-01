using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Blog.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = false; // Admin moderation
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public List<Comment> Comments { get; set; } = new();
        public List<Like> Likes { get; set; } = new();
    }
}
