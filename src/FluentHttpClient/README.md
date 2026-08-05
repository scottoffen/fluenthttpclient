# FluentHttpClient

FluentHttpClient adds a chainable API on top of [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient). You configure a request, send it, and read the response in one expression, instead of building an `HttpRequestMessage`, checking the status code, and deserializing by hand every time.

It works with the `HttpClient` you already have rather than replacing it. Each request is built on its own without changing the client or its shared configuration, so your existing setup, including `IHttpClientFactory` and typed clients, is unaffected.

## What You Get

- **Fluent configuration** of headers, query parameters, cookies, authentication, options, content, and content buffering, all in one readable chain.
- **JSON and XML** serialization and deserialization, with `JsonTypeInfo<T>` overloads for trim-safe and Native AOT scenarios.
- **Conditional configuration** that applies immediately or defers until the request is built, so you can branch without breaking the chain.
- **Response handlers** that attach success and failure callbacks inline, without interrupting the chain.
- **Extensible by subclassing**: derive from `HttpRequestBuilder` to create a custom builder shaped for a specific API or concern. Your methods chain alongside the built-in ones, and an override of `SendAsync` applies your logic to every request, since every other member on the class feeds into it.

> **Note:** FluentHttpClient has always included the ability to use custom HTTP verbs when sending requests. As of 5.1.0, we've added a dedicated `QueryAsync` method family, mirroring `GetAsync`, `PostAsync`, and the rest with the same four overloads, for the QUERY verb defined in [RFC 10008](https://datatracker.ietf.org/doc/html/rfc10008).

## Side-by-Side

The same request, written with raw `HttpClient` and with FluentHttpClient. Both deserialize the response into the same model:

```csharp
public class Post
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
}
```

### Raw HttpClient

```csharp
using System.Net.Http.Json;

var client = new HttpClient
{
    BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
};

var request = new HttpRequestMessage(HttpMethod.Get, "/posts/1");
request.Headers.Add("X-Correlation-Id", correlationId);

var response = await client.SendAsync(request);

Post? post = null;
if (response.IsSuccessStatusCode)
{
    post = await response.Content.ReadFromJsonAsync<Post>();
    Console.WriteLine($"Success: {response.StatusCode}");
}
else
{
    Console.WriteLine($"Failed: {response.StatusCode}");
}
```

### FluentHttpClient

```csharp
using FluentHttpClient;

var client = new HttpClient
{
    BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
};

var post = await client
    .UsingRoute("/posts/1")
    .WithHeader("X-Correlation-Id", correlationId)
    .GetAsync()
    .OnSuccess(r => Console.WriteLine($"Success: {r.StatusCode}"))
    .OnFailure(r => Console.WriteLine($"Failed: {r.StatusCode}"))
    .ReadJsonAsync<Post>();
```

The FluentHttpClient version expresses the same logic in fewer lines, and keeps configuration, sending, error handling, and deserialization together in one chain.

## Target Frameworks

FluentHttpClient multitargets .NET Standard 2.0 and 2.1, and .NET 6, 7, 8, 9, and 10. Through .NET Standard 2.0 it also runs on .NET Framework 4.6.1 and later. The assemblies are strong-named, and the package is Native AOT compatible when you use the `JsonTypeInfo<T>` JSON overloads.

### .NET Standard Consumers

.NET Standard 2.0 and 2.1 do not ship `System.Text.Json`, and FluentHttpClient does not bring it in transitively. If you target either one, or any other framework that does not include `System.Text.Json`, add an explicit package reference: at least `4.6.0` for `netstandard2.0` or `6.0.10` for `netstandard2.1`. A newer version is always preferable. Apps on .NET 5 and later already include it and need no extra step.

## Documentation

Full documentation, including how to create custom builders, is at https://scottoffen.github.io/fluenthttpclient.