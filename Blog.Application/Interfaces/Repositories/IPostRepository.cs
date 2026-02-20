using Blog.Application.DTOs;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repositories
{
    public interface IPostRepository
    {
        Task ApprovePostAsync(int id, bool isApproved = false);

        Task<int> CreateCommentAsync(Comment commentData);

        Task<int> CreateLikeAsync(Like like);

        Task<int> CreatePostAsync(Post postData);

        Task<bool> DeletePostAsync(int id);

        Task<List<Post>> GetPendingPostsAsync();

        Task<List<Post>> GetPostsAsync(string status);
        Task<List<Post>> GetPostsByUserIdAsync(int userId);
        Task<PostDto?> GetPostByIdAsync(int id);

        Task<bool> UpdatePostAsync(Post postData);
    }
}
