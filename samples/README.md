# FluentHttpClient Samples

This folder contains minimal, runnable console applications demonstrating common FluentHttpClient usage patterns. Each sample is self-contained and can be run with `dotnet run`.

## Running a Sample

```bash
cd sample/BasicGetSample
dotnet run
```

## Available Samples

### BasicGetSample
Demonstrates the simplest possible GET request using FluentHttpClient.

**What it shows:**
- Creating an `HttpClient` with a base address
- Using `UsingRoute()` to specify an endpoint
- Performing a GET request with `GetAsync()`
- Reading the response as a string

**Run:**
```bash
cd sample/BasicGetSample && dotnet run
```

---

### JsonPostSample
Shows how to POST JSON content to an API.

**What it shows:**
- Creating JSON content from an anonymous object
- Using `WithJsonContent()` to attach JSON to the request
- Sending a POST request with `PostAsync()`
- Reading the response body

**Run:**
```bash
cd sample/JsonPostSample && dotnet run
```

---

### WithQueryParametersSample
Demonstrates adding query string parameters to a request.

**What it shows:**
- Using `WithQueryParameter()` to add query parameters
- Automatic URL encoding and query string construction
- Retrieving filtered results from an API

**Run:**
```bash
cd sample/WithQueryParametersSample && dotnet run
```

---

### JsonDeserializeSample
Shows strongly-typed JSON deserialization using FluentHttpClient's built-in extensions.

**What it shows:**
- Defining a model class for deserialization
- Using `ReadJsonAsync<T>()` to deserialize JSON responses
- Accessing typed properties from the response

**Run:**
```bash
cd sample/JsonDeserializeSample && dotnet run
```

---

### XmlDeserializeSample
Demonstrates XML deserialization from an HTTP response.

**What it shows:**
- Using `ReadXmlAsync<T>()` to deserialize XML responses
- Working with XML serialization attributes
- Handling complex XML structures with nested elements

**Run:**
```bash
cd sample/XmlDeserializeSample && dotnet run
```

---

### WithHeadersSample
Shows how to add custom headers to requests.

**What it shows:**
- Using `WithHeader()` to add custom headers
- Adding multiple headers to a single request
- Common patterns like correlation IDs

**Run:**
```bash
cd sample/WithHeadersSample && dotnet run
```

---

### AuthenticationSample
Demonstrates adding authentication headers, specifically Bearer tokens.

**What it shows:**
- Using `WithOAuthBearerToken()` for Bearer authentication
- Adding Authorization headers to requests
- Common OAuth/API authentication patterns

**Run:**
```bash
cd sample/AuthenticationSample && dotnet run
```

---

### ErrorHandlingSample
Shows how to handle success and failure responses using FluentHttpClient's response handlers.

**What it shows:**
- Using `OnSuccess()` to handle successful responses
- Using `OnFailure()` to handle failed responses
- Fluent error handling without breaking the chain

**Run:**
```bash
cd sample/ErrorHandlingSample && dotnet run
```

---

## Test API

All samples (except XML) use [JSONPlaceholder](https://jsonplaceholder.typicode.com/), a free fake REST API for testing and prototyping. No authentication or setup is required.

## Requirements

- .NET 8.0 SDK or later
- All samples reference the local `FluentHttpClient` project

## Additional Resources

- [FluentHttpClient Documentation](../docs)
- [JSONPlaceholder API Guide](https://jsonplaceholder.typicode.com/guide/)

