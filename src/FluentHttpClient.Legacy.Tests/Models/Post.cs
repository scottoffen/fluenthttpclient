namespace FluentHttpClient.Tests.Models;

public class Post
{
    public int UserId { get; set; }

    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;
}