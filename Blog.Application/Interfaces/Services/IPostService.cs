using Blog.Application.DTOs;
namespace Blog.Application.Interfaces.Services
{
    public interface IPostService
    {
        Task<List<PostDto>> GetAllAsync(); // Public view
        Task<List<PendingPostDto>> GetPendingAsync(); // Admin view
        Task CreateAsync(CreatePostDto dto, int userId);
        Task ApproveAsync(int id); // Admin
        Task DeleteAsync(int id);
        Task CommentAsync(int postId, CreateCommentDto dto, int userId);
        Task LikeAsync(int postId, int userId);
        Task<List<PostDto>> GetPostsByUserIdAsync(int userId);
    }
}
