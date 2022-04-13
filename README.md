# <img src="fluenthttpclient.png" width=25px> FluentHttpClient

FluentHttpClient exposes a set of extensions methods to make sending REST requests with `HttpClient` both readable and chainable.

## Installation

FluentHttpClient is available on [NuGet.org](https://www.nuget.org/packages/FluentHttpClient/) and can be installed using a NuGet package manager or the .NET CLI.

Powershell:
```powershell
Install-Package FluentHttpClient -Version 1.0.0
```

.NET CLI
```cmd
> dotnet add package FluentHttpClient --version 1.0.0
```

## Correctly Injecting HttpClient

The socket exhaustion problems assocaited with the incorrect usage of the `HttpClient` class in .NET applications has been [well documented](https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/). Microsoft has published [an article introducing `IHttpClientFactory`](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0), which is used to configure and create `HttpClient` instances in an app.

### References

- [You're Using HttpClient Wrong And It Is Destabilizing Your Software](https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/)
- [Make HTTP requests using IHttpClientFactory in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0)
- [Use IHttpClientFactory to implement resilient HTTP requests](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests)


## Extension Methods

Using `HttpClient` involves creating an `HttpRequestMessage`, configuring it's properties (e.g. headers, query string, route, etc.), serializing the content, and sending that request. The response then needs to be deserialized and used.

The extension methods available in this library simplifiy that process. The `UsingRoute` extension method on `HttpClient` returns a `RestRequestBuilder` object, which has extension methods on it to configure the request. It also has extension methods to send the request using different HTTP verbs, and then there are extension methods on both `HttpResponseMessage` and `Task<HttpResponseMessage>`for deserializing the content. Put another way, the extension methods fall in to three categories.

- Configuring the `HttpRequestMessage`
- Sending the `HttpRequestMessage`
- Deserializing the `HttpResponseMessage` contents

The example below proceed in that topical order.

To enjoy the benefits of using these chaning methods, you'll want to configure the request and send it all in one chain.

```csharp
var content = await _client
    .UsingRoute($"/repos/scottoffen/grapevine/issues")
    .WithQueryParam("state", "open")
    .WithQueryParam("sort", "created")
    .WithQueryParam("direction", "desc")
    .GetAsync()
    .GetResponseStreamAsync();
```

## Configure The Request

### Route (Endpoint)

Start by setting the request route using the `UsingRoute(string route)` extension method. If the `HttpClient.BaseAddress` has already been set, the value should be relative to that value. If it has not, then you should include the fully qualified domain name and full path the endpoint.

```csharp
_client.UsingRoute($"/repos/scottoffen/grapevine/issues");
```

You'll notice that this is the only extension method on `HttpClient` of those listed here. This method actually returns a `RestRequestMessageBuilder`, and all other methods below are extension methods on that class.

### Authentication

If your requests need authentication, you can easily add it using one of the three extensions methods below.

#### Basic Authentication

Basic authentication sends a Base64 encoded username and password. You can create the Base64 encoded string yourself or let the extension method do that for you.

```csharp
// Send the username and password to be concatenated and Base64 encoded
_client.WithBasicAuthentication("username", "password");

// Concat and encode yourself, and just pass in the token
var token = "dXNlcm5hbWU6cGFzc3dvcmQ=";
_client.UsingRoute("")
    .WithBasicAuthentication(token);
```

##### OAuth Authentication

```csharp
_client.UsingRoute("")
    .WithOAuthBearerToken(bearerToken);
```

#### Other Authentication Schemes

Or you can use a [different authentication scheme](https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication#authentication_schemes) by passing in the type and credentials.

```csharp
_client.UsingRoute("")
    .WithAuthentication(type, credentials);
```

### Content

You can set the content of the request by passing in a string or a pre-built [`HttpContent`](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpcontent?view=net-5.0) object.

```csharp
var json = JsonSerializer.Serialize(myObject);
_client.UsingRoute("")
    .WithContent(json);

// Send a multipart request
MultipartContent content = ...
_client.UsingRoute("")
    .WithContent(content);

// Or more specifically multipart/form-data
MultipartFormDataContent content = ...
_client.UsingRoute("")
    .WithContent(content);
```

### Cookies

You can set one or many cookies on the request.

```csharp
// Set a single cookie
_client.UsingRoute("")
    .WithCookie("key", "value");

// Set multiple cookies
var cookies = new []
{
    new KeyValuePair<string,string>("name", "John Smith"),
    new KeyValuePair<string,string>("delta", "true")
};

_client.UsingRoute("")
    .WithCookies(cookies);
```

### Headers

You can set one or many headers on the request.

```csharp
// Set a single header
_client.UsingRoute("")
    .WithHeader("X-CustomHeader", "MyCustomValue");

// Set multiple headers
var headers = new []
{
    new KeyValuePair<string,string>("Content-Type", "application/json"),
    new KeyValuePair<string,string>("Content-Encoding", "gzip")
};

_client.UsingRoute("")
    .WithHeaders(headers);
```

### Query Parameters

Add query parameters to the route.

```csharp
// Set a single query parameter
_client.UsingRoute("")
    .WithQueryParam("id", "13485");

// Set multiple query parameters
var query = new NameValueCollection()
{
    {"state", "open"},
    {"sort", "created"},
    {"direction", "desc"},
};

_client.UsingRoute("")
    .WithQueryParams(query);
```

### Timeout

You can set the timeout value for a request by passing either a number of seconds as an integer or passing in a `TimeSpan` instance.

```csharp
// Set to 10 seconds
_client.UsingRoute("")
    .WithRequestTimeout(10);

// Set to a timespan of 10 seconds
_client.UsingRoute("")
    .WithRequestTimeout(TimeSpan.FromSeconds(10));
```

## Send The Request

There are extension methods specifically for the most common HTTP methods, or you can pass in the method to be used. All of the methods take an optional cancellation token (not shown).

```csharp
var response = await _client
    .UsingRoute("/user/list")
    .WithQueryParam("sort", "desc")
    .GetAsync();

var response = await _client
    .UsingRoute("/user")
    .WithContent(userdata)
    .PostAsync();

var response = await _client
    .UsingRoute("/user/1234")
    .WithContent(userdata)
    .PutAsync();

var response = await _client
    .UsingRoute("/user/234")
    .DeleteAsync();

var response = await _client
    .UsingRoute("/user")
    .WithContent(userdata)
    .SendAsync(HttpMethod.Patch);
```

## Deserialize The Response

You can deserialize the response body to a string, stream or byte array using an extension method on the response.

### Get Response As String

```csharp {7,18}
string response = await _client
    .UsingRoute($"/repos/scottoffen/grapevine/issues")
    .WithQueryParam("state", "open")
    .WithQueryParam("sort", "created")
    .WithQueryParam("direction", "desc")
    .GetAsync()
    .GetResponseStringAsync();

/* OR */

var response = await _client
    .UsingRoute($"/repos/scottoffen/grapevine/issues")
    .WithQueryParam("state", "open")
    .WithQueryParam("sort", "created")
    .WithQueryParam("direction", "desc")
    .GetAsync();

var responseContent = response.GetResponseStringAsync();
```

### Get Response As Stream
```csharp {7,18}
string response = await _client
    .UsingRoute($"/repos/scottoffen/grapevine/issues")
    .WithQueryParam("state", "open")
    .WithQueryParam("sort", "created")
    .WithQueryParam("direction", "desc")
    .GetAsync()
    .GetResponseStreamAsync();

/* OR */

var response = await _client
    .UsingRoute($"/repos/scottoffen/grapevine/issues")
    .WithQueryParam("state", "open")
    .WithQueryParam("sort", "created")
    .WithQueryParam("direction", "desc")
    .GetAsync();

var responseContent = response.GetResponseStreamAsync();
```

### Get Response As Byte Array

```csharp {7,18}
string response = await _client
    .UsingRoute($"/repos/scottoffen/grapevine/issues")
    .WithQueryParam("state", "open")
    .WithQueryParam("sort", "created")
    .WithQueryParam("direction", "desc")
    .GetAsync()
    .GetResponseBytesAsync();

/* OR */

var response = await _client
    .UsingRoute($"/repos/scottoffen/grapevine/issues")
    .WithQueryParam("state", "open")
    .WithQueryParam("sort", "created")
    .WithQueryParam("direction", "desc")
    .GetAsync();

var responseContent = response.GetResponseBytesAsync();
```