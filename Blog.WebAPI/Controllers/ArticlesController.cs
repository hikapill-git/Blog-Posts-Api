using Blog.Application.DTOs;
using Blog.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IPostService _postService;
        public ArticlesController(IPostService _postService)
        {
            this._postService = _postService;
        }
        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<List<PostDto>>> GetAll()
        {
            return Ok(await _postService.GetAllAsync());
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create(CreatePostDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _postService.CreateAsync(dto, userId);
            return Ok();
        }
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> Update(CreatePostDto dto, int postId)
        {
            await _postService.UpdateAsync(dto, postId);
            return Ok();
        }

        [HttpGet("Pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<PendingPostDto>>> Pending([FromQuery] string status = "all")
        {
            return Ok(await _postService.GetPendingAsync(status));
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PostDto>> GetPostById(int id)
        {
            return Ok(await _postService.GetPostByIdAsync(id));
        }
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<List<PostDto>>> GetPostsByUserId(int userId)
        {
            return Ok(await _postService.GetPostsByUserIdAsync(userId));
        }
        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Approve(int id, [FromQuery] bool status)
        {
            await _postService.ApproveAsync(id, status);
            return Ok();
        }
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            await _postService.DeleteAsync(id);
            return Ok();
        }
        [HttpPost("Comment")]
        [Authorize]
        public async Task<ActionResult> Comment(CreateCommentDto dto, int postId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _postService.CommentAsync(postId, dto, userId);
            return Ok();
        }
        [HttpPost("Like")]
        [Authorize]
        public async Task<ActionResult> Like(int postId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _postService.LikeAsync(postId, userId);
            return Ok();
        }
    }
}
