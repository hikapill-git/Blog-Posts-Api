using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string EmailId { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // "Admin" or "User"
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public List<Post> Posts { get; set; } = new();
        
    }
}
