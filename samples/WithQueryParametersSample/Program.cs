using FluentHttpClient;

var client = new HttpClient
{
    BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
};

var json = await client
    .UsingRoute("/comments")
    .WithQueryParameter("postId", "1")
    .GetAsync()
    .ReadContentAsStringAsync();

Console.WriteLine(json);
