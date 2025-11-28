using FluentHttpClient;

var client = new HttpClient
{
    BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
};

var token = "demo-token-12345";

var json = await client
    .UsingRoute("/posts/1")
    .WithOAuthBearerToken(token)
    .GetAsync()
    .ReadContentAsStringAsync();

Console.WriteLine(json);
