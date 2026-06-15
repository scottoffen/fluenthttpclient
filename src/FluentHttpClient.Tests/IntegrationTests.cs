namespace FluentHttpClient.Tests;

public class IntegrationTests
{
    [Fact]
    public async Task ReadJsonAsync_ShouldReturnDeserializedObjectMultipleTimes()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
        };

        var response = await client
            .UsingRoute("posts/1")
            .GetAsync();

        var post1 = await response.ReadJsonAsync<Post>();
        var post2 = await response.ReadJsonAsync<Post>();

        post1.ShouldNotBeNull();
        post2.ShouldNotBeNull();
    }
}

public sealed class Post
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
}