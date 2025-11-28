using FluentHttpClient;

var client = new HttpClient
{
    BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
};

var json = await client
    .UsingRoute("/posts/1")
    .WithHeader("X-Custom-Header", "demo-value")
    .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString())
    .GetAsync()
    .ReadContentAsStringAsync();

Console.WriteLine(json);
