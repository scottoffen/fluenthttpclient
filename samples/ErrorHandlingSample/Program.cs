using FluentHttpClient;

var client = new HttpClient
{
    BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
};

var json = await client
    .UsingRoute("/posts/999")
    .GetAsync()
    .OnSuccess(r => Console.WriteLine($"Success! Status: {r.StatusCode}"))
    .OnFailure(r => Console.WriteLine($"Failed! Status: {r.StatusCode}"))
    .ReadContentAsStringAsync();

Console.WriteLine(json);
