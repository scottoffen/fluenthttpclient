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

If you are building against **netstandard2.0** or **netstandard2.1**, or any TFM that does **not** ship `System.Text.Json`, you will need to add an explicit package reference with a minimum version of 6.0.10 (a higher version is always recommended):

```xml
<PackageReference Include="System.Text.Json" Version="6.0.10" />
```

Apps targeting modern TFMs (such as .NET 5 and later) already include `System.Text.Json` and do not require this step.

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

FluentHttpClient can expresses the same logic in fewer lines, with better readability and no loss of functionality. All configuration, sending, error handling, and deserialization happen in a single fluent chain.