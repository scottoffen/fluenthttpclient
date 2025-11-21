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