# FluentHttpClient

[![docs](https://img.shields.io/badge/docs-github.io-blue)](https://scottoffen.github.io/fluenthttpclient)
[![NuGet](https://img.shields.io/nuget/v/fluenthttpclient)](https://www.nuget.org/packages/FluentHttpClient/)
[![MIT](https://img.shields.io/github/license/scottoffen/fluenthttpclient?color=blue)](./LICENSE)
[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.1-blue.svg)](code_of_conduct.md)
[![FluentHttpClient](https://img.shields.io/badge/FluentHttpClient-strong%20named-ff8038.svg)](https://learn.microsoft.com/dotnet/standard/assembly/strong-named)
[![Multi-targeted](https://img.shields.io/badge/TFMs-multi--targeted-652f94)](#compatibility-matrix)

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

## Installation

FluentHttpClient is available on [NuGet.org](https://www.nuget.org/packages/FluentHttpClient/) and can be installed using a NuGet package manager or the .NET CLI.

## When to Use FluentHttpClient

While `HttpClient` is a powerful and flexible tool, building HTTP requests with it often involves repetitive boilerplate, manual serialization, and scattered configuration logic. FluentHttpClient addresses these pain points by providing a fluent, chainable API that reduces cognitive load and improves code readability.

### Common HttpClient Challenges

**Repetitive Configuration**  
Every request requires manually setting headers, query parameters, and content, often scattered across multiple lines. This makes it easy to miss required headers or forget encoding rules.

**Manual Serialization**  
Converting objects to JSON, setting the correct `Content-Type`, and deserializing responses requires multiple steps and imports. Error-prone encoding and parsing logic often needs to be duplicated across your codebase.

**Inconsistent Error Handling**  
Without a unified approach to handling success and failure responses, status code checks and logging logic tend to be duplicated or omitted entirely.

**Lifetime and Reuse Concerns**  
Properly managing `HttpClient` lifetime, avoiding socket exhaustion, and reusing instances while still configuring per-request state requires careful planning and often leads to awkward patterns.

### How FluentHttpClient Helps

FluentHttpClient wraps `HttpClient` (you still manage the lifetime) and provides extension methods that let you configure requests in a single, readable chain:

- **Fluent Configuration**: Add headers, query parameters, cookies, and authentication in a natural, discoverable flow
- **Automatic Serialization**: Built-in JSON and XML serialization/deserialization with support for `System.Text.Json`, Native AOT, and custom options
- **Response Handlers**: Attach success and failure callbacks directly in the request chain without breaking fluency
- **Reduced Boilerplate**: Express the entire request lifecycle—configuration, sending, and deserialization—in a single expression

### Side-by-Side Comparison

Here's the same request implemented with raw `HttpClient` and FluentHttpClient:

#### Raw HttpClient

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

public class Post
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
}
```

#### FluentHttpClient

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

public class Post
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
}
```

Because a fluent API improves developer experience by turning tedious, repetitive setup into a readable, chainable flow that matches how you actually think about building and sending an HTTP request, The FluentHttpClient version expresses the same logic in fewer lines, with better readability and no loss of functionality. All configuration, sending, error handling, and deserialization happen in a single fluent chain.

## Usage and Support

- Check out the project documentation https://scottoffen.github.io/fluenthttpclient.

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

Few thing are as satisfying as hearing that your open source project is being used and appreciated by others. Jump over to the discussion boards and [share the love](https://github.com/scottoffen/fluenthttpclient/discussions)!
