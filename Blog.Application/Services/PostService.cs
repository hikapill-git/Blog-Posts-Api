using Blog.Application.DTOs;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Entities;

namespace Blog.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository repository;

        public PostService(IPostRepository repository)
        {
            this.repository = repository;
        }

        public async Task ApproveAsync(int id)
        {
            await repository.ApprovePostAsync(id, true);
        }

        public async Task CommentAsync(int postId, CreateCommentDto dto, int userId)
        {
            Comment comment = new Comment
            {
                Text = dto.Text,
                UserId = userId,
                PostId = postId
            };
            await repository.CreateCommentAsync(comment);
        }

        public async Task CreateAsync(CreatePostDto dto, int userId)
        {
            Post post = new Post
            {
                UserId = userId,
                Content = dto.Content,
                Title = dto.Title,
            };
            await repository.CreatePostAsync(post);
        }

        public async Task DeleteAsync(int id)
        {
            await repository.DeletePostAsync(id);
        }

        public async Task<List<PostDto>> GetAllAsync()
        {
            var posts = await repository.GetPostsAsync();
            return posts.Select(p => new PostDto(
                    p.Id,
                    p.Title,
                    p.Content,
                    p.IsApproved,
                    p.Likes.Count(),
                    p.Comments.Select(c => new CommentDto(
                        c.Id,
                        c.Text,
                        p.User.FName + " " + p.User.LName
                    )).ToList()
                )).ToList();
        }

        public async Task<List<PendingPostDto>> GetPendingAsync()
        {
            var posts = await repository.GetPostsAsync();
            return posts.Select(p => new PendingPostDto(
                    p.Id,
                    p.Title,
                    p.Content,
                    p.IsApproved,
                    p.User.EmailId,
                    p.User.FName,
                    p.User.LName
                )).ToList();
        }

        public async Task LikeAsync(int postId, int userId)
        {
            Like like = new Like { UserId = userId, PostId = postId };
            await repository.CreateLikeAsync(like);
        }
        public async Task<List<PostDto>> GetPostsByUserIdAsync(int userId)
        {
            var posts = await repository.GetPostsByUserIdAsync(userId);
            return posts.Select(p => new PostDto(
                    p.Id,
                    p.Title,
                    p.Content,
                    p.IsApproved,
                    0,
                    new List<CommentDto>()
                )).ToList();
        }
        public async Task<PostDto> GetPostByIdAsync(int id) 
        {
            var posts = await repository.GetPostByIdAsync(id);
            PostDto dto = new PostDto(
                    0,
                    string.Empty,
                    string.Empty,
                    false,
                    0,
                    new List<CommentDto>()
                );
            if (posts != null)
            {
               dto = new PostDto(
                    posts.Id,
                    posts.Title,
                    posts.Content,
                    posts.IsApproved,
                    posts.Likes.Count(),
                    posts.Comments.Select(c => new CommentDto(
                        c.Id,
                        c.Text,
                        posts.User.FName + " " + posts.User.LName
                    )).ToList()
                );
            }
            return dto;

        }
    }
}
