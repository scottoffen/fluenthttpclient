using FluentHttpClient;
using System.Text.Json;

var client = new HttpClient
{
    BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
};

var newPost = new
{
    title = "FluentHttpClient Demo",
    body = "Testing POST with JSON content",
    userId = 1
};

var response = await client
    .UsingRoute("/posts")
    .WithJsonContent(newPost)
    .PostAsync()
    .ReadContentAsStringAsync();

Console.WriteLine(response);
