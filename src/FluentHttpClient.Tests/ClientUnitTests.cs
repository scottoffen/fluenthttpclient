using FluentHttpClient.Tests.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentHttpClient.Tests;

public class ClientUnitTests : UnitTestBase
{
    private readonly HttpClient _client;

    private readonly int MaxPostId = 100;
    private readonly int MinPostId = 1;
    private readonly int MaxUserId = 10;
    private readonly int MinUserId = 1;
    private readonly int PostsPerUser = 10;

    public ClientUnitTests()
    {
        _client = GetRequiredService<HttpClient>();
        _client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
    }

    protected override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
    }

    [Fact]
    public async Task OnFailureTest()
    {
        var route = $"/posts";
        var passed = false;

        await _client.UsingRoute(route)
            .DeleteAsync()
            .OnFailure(msg =>
            {
                passed = true;
            })
            .OnSuccess(msg =>
            {
                throw new Exception("The request returned a success status code.");
            });

        passed.ShouldBeTrue();
    }

    [Fact]
    public async Task OnSuccessTest()
    {
        var postId = Random.Shared.Next(MinPostId, MaxPostId);
        var route = $"/posts/{postId}";
        var passed = false;

        await _client.UsingRoute(route)
            .DeleteAsync()
            .OnFailure(msg =>
            {
                throw new Exception("The request did not return a success status code.");
            })
            .OnSuccess(msg =>
            {
                passed = true;
            });

        passed.ShouldBeTrue();
    }

    [Fact]
    public async Task DefaultActionTest()
    {
        var route = $"/post/1";
        var passed = false;

        var builder = _client.UsingRoute(route);

        var response = await builder
            .GetAsync();

        var post1 = await response.DeserializeJsonAsync<Post>();
        var post2 = await response.DeserializeJsonAsync<Post>((msg, ex) =>
            {
                passed = true;
                return (Post)null;
            });

        post1.ShouldNotBeNull();
        post2.ShouldBeNull();
        passed.ShouldBeTrue();
    }

    [Fact]
    public async Task GetAllAsyncTest()
    {
        var route = "/posts";

        var builder = _client.UsingRoute(route);

        var response = await builder.GetAsync();

        response.EnsureSuccessStatusCode();

        var posts = await response
            .DeserializeJsonAsync<IEnumerable<Post>>();

        posts.ShouldNotBeNull();
        posts.ShouldNotBeEmpty();
        posts.Count().ShouldBe(MaxPostId);
    }

    [Fact]
    public async Task GetPostsForUserAsyncTest()
    {
        var userId = Random.Shared.Next(MinUserId, MaxUserId);
        var route = $"/posts";

        var builder = _client.UsingRoute(route);

        builder.WithQueryParam("userId", userId);

        var response = await builder.GetAsync();

        response.EnsureSuccessStatusCode();

        var posts = await response
            .DeserializeJsonAsync<IEnumerable<Post>>();

        posts.ShouldNotBeNull();
        posts.ShouldNotBeEmpty();
        posts.Count().ShouldBe(PostsPerUser);
        posts.All(x => x.UserId == userId).ShouldBeTrue();
    }

    [Fact]
    public async Task GetPostByIdAsyncTest()
    {
        var postId = Random.Shared.Next(MinPostId, MaxPostId);
        var route = $"/posts/{postId}";

        var builder = _client.UsingRoute(route);

        var response = await builder.GetAsync();

        response.EnsureSuccessStatusCode();

        var post = await response
            .DeserializeJsonAsync<Post>();

        post.ShouldNotBeNull();
        post.Id.ShouldBe(postId);
    }

    [Fact]
    public async Task DeletePostAsync()
    {
        var postId = Random.Shared.Next(MinPostId, MaxPostId);
        var route = $"/posts/{postId}";

        var builder = _client.UsingRoute(route);

        var response = await builder.DeleteAsync();

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreatePostAsync()
    {
        var post = new Post
        {
            Id = 0,
            UserId = Random.Shared.Next(MinUserId, MaxUserId),
            Title = Guid.NewGuid().ToString(),
            Body = Guid.NewGuid().ToString()
        };

        var newPost = await _client
            .UsingRoute("/posts")
            .WithJsonContent(post)
            .PostAsync()
            .DeserializeJsonAsync<Post>();

        newPost.ShouldNotBeNull();
        newPost.ShouldSatisfyAllConditions
        (
            () => newPost.Id.ShouldBe(MaxPostId + 1),
            () => newPost.UserId.ShouldBe(post.UserId),
            () => newPost.Title.ShouldBe(post.Title),
            () => newPost.Body.ShouldBe(post.Body)
        );
    }

    [Fact]
    public async Task UpdatePostAsync()
    {
        var expected = new Post
        {
            Id = Random.Shared.Next(MinPostId, MaxPostId),
            UserId = Random.Shared.Next(MinUserId, MaxUserId),
            Title = Guid.NewGuid().ToString(),
            Body = Guid.NewGuid().ToString()
        };

        var route = $"/posts/{expected.Id}";
        var builder = _client.UsingRoute(route);

        builder.WithJsonContent(expected);

        var response = await builder.PutAsync();

        response.EnsureSuccessStatusCode();

        var actual =  await response
            .DeserializeJsonAsync<Post>();

        actual.ShouldNotBeNull();
        actual.ShouldSatisfyAllConditions
        (
            () => actual.Id.ShouldBe(expected.Id),
            () => actual.UserId.ShouldBe(expected.UserId),
            () => actual.Title.ShouldBe(expected.Title),
            () => actual.Body.ShouldBe(expected.Body)
        );
    }
}
