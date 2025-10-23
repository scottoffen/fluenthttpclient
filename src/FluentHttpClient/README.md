# FluentHttpClient

FluentHttpClient provides a fluent, composable API for building, sending, and handling HTTP requests and responses in .NET. It extends `HttpClient` with a modern, intuitive syntax while remaining unopinionated and lightweight.

## Features

* Fluent, chainable request configuration
* Strongly typed request and response helpers
* Built-in JSON and XML serialization and deserialization
* Automatic handling of query parameters, headers, and cookies
* Extensible via custom configuration and options
* Compatible with .NET 6.0 and later

## Example

```csharp
HttpClient client = new HttpClient{ BaseAddress = new Uri("https://www.example.com") };

var user = await client
    .UsingRoute("/users/123")
    .WithOAuthBearerToken(token)
    .GetAsync()
    .DeserializeJsonAsync<User>();
```

## Why FluentHttpClient

FluentHttpClient simplifies the repetitive patterns of `HttpClient` usage while remaining transparent.
It allows developers to build expressive request pipelines that integrate easily with existing Polly policies, dependency injection configurations, and platform features.

## Documentation

Full documentation, examples, and API reference are available on the project site or in the repository’s
