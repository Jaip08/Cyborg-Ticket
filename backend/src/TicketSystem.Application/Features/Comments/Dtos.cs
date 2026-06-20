namespace TicketSystem.Application.Features.Comments;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public bool IsInternal { get; set; }
    public DateTime CreatedAt { get; set; }
    public CommentAuthorDto Author { get; set; } = default!;
}

public class CommentAuthorDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Role { get; set; } = default!;
}
