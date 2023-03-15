using System.Collections.Specialized;
using FluentHttpClient;

HttpClient client = new()
{
    BaseAddress = new Uri("https://api.github.com/")
};

client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
client.DefaultRequestHeaders.Add("User-Agent", "Fluent-Http-Client");

var owner = "scottoffen";
var repo = "fluenthttpclient";
var query = new NameValueCollection()
{
    {"state", "open"},
    {"sort", "created"},
    {"direction", "desc"},
};

var issues = await client
    .UsingRoute($"/repos/{owner}/{repo}/issues")
    .WithQueryParams(query)
    .GetAsync()
    .OnFailure(msg => { Console.WriteLine("Failure"); })
    .OnSuccess(msg => { Console.WriteLine("Success"); })
    .DeserializeJsonAsync<Issues>();

Console.WriteLine();
Console.WriteLine("----------------------------------------------");

foreach (var issue in issues)
{
    Console.WriteLine($"{issue.Id} : {issue.Title}");
}

Console.WriteLine("----------------------------------------------");
Console.WriteLine();

public class Issues : List<Issue> { }

public class Issue
{
    public int Id { get; set; }
    public string Title { get; set; }
}