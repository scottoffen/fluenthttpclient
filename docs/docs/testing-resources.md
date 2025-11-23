---
sidebar_position: 15
title: Testing Resources
---

When you're exploring FluentHttpClient for the first time, it's helpful to have a simple, predictable API to call without needing to stand up your own backend. A great option for this is **JSONPlaceholder**, a free, public REST API designed specifically for testing and prototyping:

**[https://jsonplaceholder.typicode.com](https://jsonplaceholder.typicode.com)**

JSONPlaceholder provides stable, well-defined endpoints for common resources (posts, comments, users, etc.). Since these endpoints always return consistent data and never modify real state, they're perfect for demonstrating FluentHttpClient's request building workflow and testing different serialization patterns.

**Why JSONPlaceholder Works Well with FluentHttpClient**

* It offers real HTTP responses - no mocks required.
* You can test GET, POST, PUT, PATCH, and DELETE requests safely.
* It accepts JSON input and returns JSON output
* You can experiment without worrying about breaking anything.

## Basic Usage

Below is an example that retrieves a post, creates a new one, and deserializes JSON responses into strongly-typed models.

```csharp
public sealed class Post
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
}
```

### Fetching a Single Post

```csharp
var client = new HttpClient
{
    BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
};

var post = await client
    .UsingBase()
    .WithRoute("posts/1")
    .SendAsync()
    .ReadJsonAsync<Post>();

Console.WriteLine(post?.Title);
```

### Fetching a List of Posts

```csharp
var posts = await client
    .UsingBase()
    .WithRoute("posts")
    .SendAsync()
    .ReadJsonAsync<List<Post>>();

Console.WriteLine(posts?.Count);
```

### Creating a Post

JSONPlaceholder won't actually store your changes, but it simulates success and returns the created object.

```csharp
var newPost = new Post
{
    UserId = 1,
    Title = "Hello FluentHttpClient",
    Body = "Testing JSONPlaceholder"
};

var created = await client
    .UsingBase()
    .WithRoute("posts")
    .WithJsonContent(newPost)
    .PostAsync()
    .ReadJsonAsync<Post>();

Console.WriteLine(created?.Id);
```

## Trying Other Endpoints

JSONPlaceholder supports additional routes like:

* `/comments`
* `/albums`
* `/photos`
* `/todos`
* `/users`

All of them work the same way with FluentHttpClient - configure, send, and deserialize.
