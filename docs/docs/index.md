---
sidebar_position: 0
title: Introduction
---

# FluentHttpClient

FluentHttpClient brings a modern, chainable API to [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient), turning verbose request setup into clean, expressive fluency. It handles headers, options, cookies, query parameters, conditional configurators, buffering, and *both* JSON/XML serialization and deserialization, along with success and failure handlers, all with minimal ceremony. It multitargets from **.NET Standard 2.0** all the way up through **.NET 10**, giving you broad compatibility across older runtimes and the latest platforms, with full Native AOT compatibility and strong-named assemblies.

## Compatibility Matrix

FluentHttpClient is optimized for .NET 10 and the newest .NET releases, while also supporting older platforms through .NET Standard 2.1 and 2.0 for teams maintaining long-lived or legacy applications. It includes full Native AOT compatibility and provides strong-named assemblies for environments that require them.

| Target                    | Supported | Notes                         |
| ------------------------- | --------- | ----------------------------- |
| **.NET Standard 2.0**     | ✔️        | Broadest compatibility target |
| **.NET Standard 2.1**     | ✔️        | Improved modern API surface   |
| **.NET Framework 4.6.1+** | ✔️        | Via `netstandard2.0`          |
| **.NET 6**                | ✔️        | LTS                           |
| **.NET 7**                | ✔️        |                               |
| **.NET 8**                | ✔️        | LTS                           |
| **.NET 9**                | ✔️        |                               |
| **.NET 10**               | ✔️        | LTS                           |

### .NETStandard Consumers

Projects targeting **.NETStandard 2.0** or **.NETStandard 2.1** do not include `System.Text.Json` in the framework. FluentHttpClient uses `System.Text.Json` internally for its JSON extensions, but the package is not referenced transitively.

If you are building against **netstandard2.0** or **netstandard2.1**, or any TFM that does **not** ship `System.Text.Json`, you will need to add an explicit package reference, with a minimum version of 4.6.0 or 6.0.10, respectively. A higher version is always recommended.

Apps targeting modern TFMs (such as .NET 5 and later) already include `System.Text.Json` and do not require this step.

## A Better Way to Send HTTP Requests

FluentHttpClient is built around the way you actually write HTTP code: configure the request, send it, and handle/deserialize the response. Instead of scattering headers, query parameters, content, and deserialization across multiple calls and helper classes, you express the whole flow as a single, readable chain that sits naturally on top of `HttpClient`.

- **Configure** - Start from `HttpClient` and fluently add headers, cookies, options, query parameters, and content (string, JSON, XML, form data), with optional conditional configurators for late-bound values.

- **Send** - Use a focused `SendAsync` extension that builds the `HttpRequestMessage`, applies deferred configuration, and sends the request using your existing `HttpClient` instance and lifetime management.

- **Handle Responses** - Use `OnSuccess` and `OnFailure` to branch based on status codes without cluttering your code with manual checks or repeated boilerplate.

- **Deserialize** - Handle responses with extensions for reading content (string, bytes, stream) and strongly-typed JSON/XML deserialization, so the last step in your chain gives you the shape you actually care about.

```csharp
var httpClient = new HttpClient();

var weather = await httpClient
    .UsingRoute("https://api.example.com/weather")
    .WithQueryParameter("city", "Denver")
    .WithHeader("X-Correlation-Id", correlationId)
    .When(_ => isPreviewEnvironment, b => b.WithQueryParameter("preview", "true"))
    .GetAsync()
    .OnSuccess(r => logger.LogInformation("Success: {Status}", r.StatusCode))
    .OnFailure(r => logger.LogWarning("Failed: {Status}", r.StatusCode))
    .ReadJsonAsync<WeatherForecast>();
```