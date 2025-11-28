using FluentHttpClient;

var client = new HttpClient
{
    BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
};

var user = await client
    .UsingRoute("/users/1")
    .GetAsync()
    .ReadJsonAsync<User>();

Console.WriteLine($"ID: {user?.Id}");
Console.WriteLine($"Name: {user?.Name}");
Console.WriteLine($"Email: {user?.Email}");
Console.WriteLine($"Phone: {user?.Phone}");
Console.WriteLine($"Website: {user?.Website}");

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
}
