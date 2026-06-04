# FluentHttpClient

[![docs](https://img.shields.io/badge/docs-github.io-blue)](https://scottoffen.github.io/fluenthttpclient)
[![NuGet](https://img.shields.io/nuget/v/fluenthttpclient)](https://www.nuget.org/packages/FluentHttpClient/)
[![MIT](https://img.shields.io/github/license/scottoffen/fluenthttpclient?color=blue)](./LICENSE)
[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.1-blue.svg)](CODE_OF_CONDUCT.md)
[![FluentHttpClient](https://img.shields.io/badge/FluentHttpClient-strong%20named-ff8038.svg)](https://learn.microsoft.com/dotnet/standard/assembly/strong-named)
[![Multi-targeted](https://img.shields.io/badge/TFMs-multi--targeted-652f94)](#target-frameworks)

FluentHttpClient adds a chainable API on top of [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient). You configure a request, send it, and read the response in one expression, instead of building an `HttpRequestMessage`, checking the status code, and deserializing by hand every time.

It works with the `HttpClient` you already have rather than replacing it. Each request is built on its own without changing the client or its shared configuration, so your existing setup, including `IHttpClientFactory` and typed clients, is unaffected.

## What You Get

- **Fluent configuration** of headers, query parameters, cookies, authentication, options, content, and content buffering, all in one readable chain.
- **JSON and XML** serialization and deserialization, with `JsonTypeInfo<T>` overloads for trim-safe and Native AOT scenarios.
- **Conditional configuration** that applies immediately or defers until the request is built, so you can branch without breaking the chain.
- **Response handlers** that attach success and failure callbacks inline, without interrupting the chain.
- **Extensible by subclassing**: derive from `HttpRequestBuilder` to create a custom builder shaped for a specific API or concern. Your methods chain alongside the built-in ones, and an override of `SendAsync` applies your logic to every request, since every other member on the class feeds into it.

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

## Community and Support

- Engage in our [community discussions](https://github.com/scottoffen/fluenthttpclient/discussions) for Q&A, ideas, and show and tell!

- **Issues created to ask "how to" questions will be closed.**

## Contributing

We welcome contributions from the community! In order to ensure the best experience for everyone, before creating an issue or submitting a pull request, please see the [contributing guidelines](CONTRIBUTING.md) and the [code of conduct](CODE_OF_CONDUCT.md). Failure to adhere to these guidelines can result in significant delays in getting your contributions included in the project.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/scottoffen/fluenthttpclient/releases).

## Test Coverage

You can generate and open a test coverage report by running the following command in the project root:

```bash
pwsh ./test-coverage.ps1
```

> [!NOTE]
> This is a [Powershell](https://learn.microsoft.com/en-us/powershell/) script. You must have Powershell installed to run this command.

## License

FluentHttpClient is licensed under the [MIT](./LICENSE) license.

## Using FluentHttpClient? We'd Love To Hear About It!

Few things are as satisfying as hearing that your open source project is being used and appreciated by others. Jump over to the discussion boards and [share the love](https://github.com/scottoffen/fluenthttpclient/discussions)!