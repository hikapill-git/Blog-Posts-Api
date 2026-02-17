using Blog.Application.DTOs;
using Blog.Domain.Entities;
namespace Blog.Application.Interfaces.Services
{
    public interface IPostService
    {
        Task<List<PostDto>> GetAllAsync(); // Public view
        Task<List<PendingPostDto>> GetPendingAsync(string status); // Admin view
        Task CreateAsync(CreatePostDto dto, int userId);
        Task ApproveAsync(int id, bool status); // Admin
        Task DeleteAsync(int id);
        Task CommentAsync(int postId, CreateCommentDto dto, int userId);
        Task LikeAsync(int postId, int userId);
        Task<List<PostDto>> GetPostsByUserIdAsync(int userId);
        Task<PostDto> GetPostByIdAsync(int id);
        Task<bool> UpdateAsync(CreatePostDto postData, int postId);
    }
}
