# FluentHttpClient

[![docs](https://img.shields.io/badge/docs-github.io-blue)](https://scottoffen.github.io/fluenthttpclient)
[![NuGet](https://img.shields.io/nuget/v/fluenthttpclient)](https://www.nuget.org/packages/FluentHttpClient/)
[![MIT](https://img.shields.io/github/license/scottoffen/fluenthttpclient?color=blue)](./LICENSE)
[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.1-blue.svg)](code_of_conduct.md)
[![FluentHttpClient](https://img.shields.io/badge/FluentHttpClient-strong%20named-ff8038.svg)](https://learn.microsoft.com/dotnet/standard/assembly/strong-named)
[![Multi-targeted](https://img.shields.io/badge/TFMs-multi--targeted-652f94)](#compatibility-matrix)


FluentHttpClient exposes a set of extensions methods to make sending HTTP requests with [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) both readable and chainable.

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

## Installation

FluentHttpClient is available on [NuGet.org](https://www.nuget.org/packages/FluentHttpClient/) and can be installed using a NuGet package manager or the .NET CLI.

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
