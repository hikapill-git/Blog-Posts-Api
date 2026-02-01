namespace Blog.Application.DTOs
{
    public record PostDto(int Id, string Title, string Content, bool IsApproved, int LikesCount, List<CommentDto> Comments);
    public record PendingPostDto(int Id, string Title, string Content, bool IsApproved, string EmailId, string FName, string LName);
    public record CreatePostDto(string Title, string Content);
    public record CommentDto(int Id, string Text, string Username);
    public record CreateCommentDto(string Text);
}
