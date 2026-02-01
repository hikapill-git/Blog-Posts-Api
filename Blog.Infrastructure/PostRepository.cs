using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure
{
    public class PostRepository : IPostRepository
    {
        private readonly BlogContext _context;
        public PostRepository(BlogContext _context)
        {
            this._context = _context;
        }
        public async Task ApprovePostAsync(int id, bool isApproved = false)
        {
            Post? post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                post.IsApproved = isApproved;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CreateCommentAsync(Comment commentData)
        {
            await _context.Comments.AddAsync(commentData); 
            await _context.SaveChangesAsync();
            return commentData.Id;
        }

        public async Task<int> CreateLikeAsync(Like like)
        {
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
            return like.Id;
        }

        public async Task<int> CreatePostAsync(Post postData)
        {
            await _context.Posts.AddAsync(postData);
            await _context.SaveChangesAsync();
            return postData.Id;
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var post = new Post { Id = id };
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Post>> GetPendingPostsAsync()
        {
            return await _context.Posts
                   .Where(p => !p.IsApproved)
                   .OrderByDescending(p => p.CreatedAt)
                   .Include(p => p.User)
                   .ToListAsync();
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await _context.Posts
                   .Where(p => p.IsApproved)
                   .OrderByDescending(p => p.CreatedAt)
                   .Include(p => p.Comments)          // load related comments
                    .Include(p => p.Likes)     // load related likes
                    .Include(p => p.User)     // load related author profile
                   .ToListAsync();
        }
        public async Task<List<Post>> GetPostsByUserIdAsync(int userId)
        {
            return await _context.Posts
                   .Where(p => p.UserId == userId)
                   .OrderByDescending(p => p.CreatedAt)
                   //.Include(p => p.Comments)          // load related comments
                   // .Include(p => p.Likes)     // load related likes
                   // .Include(p => p.User)     // load related author profile
                   .ToListAsync();
        }
    }
}
