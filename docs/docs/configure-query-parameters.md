---
sidebar_position: 3
title: Configure Query Parameters
---

FluentHttpClient provides a flexible set of extension methods for adding query string parameters to an `HttpRequestBuilder` instance. All query parameters accumulate on `HttpRequestBuilder.QueryParameters` and are applied when the final `HttpRequestMessage` is constructed.

## Single Parameters

Use `WithQueryParameter` when you want to append one value for a given key.

### String input

```csharp
var builder = client
    .UsingBase()
    .WithQueryParameter("search", "dinosaurs");
```

* Adds a single value.
* Throws if `key` is `null`.
* Allows null values, which serialize as `?key` (flag-style parameter).
* Allows empty strings, which serialize as `?key=`.

### Object input

```csharp
var builder = client
    .UsingBase()
    .WithQueryParameter("page", 3);
```

* Calls `value?.ToString()`.
* Useful when calling code already has boxed values.

## Multiple Values for Single Key

Use these overloads when you want repeated keys in the final query string.

### String Sequences

```csharp
var builder = client
    .UsingBase()
    .WithQueryParameter("category", new[] { "raptors", "ceratopsians" });
// produces: ?category=raptors&category=ceratopsians
```

* Throws if `key` or `values` is `null`.
* Each value is serialized as its own parameter.

### Object Sequences

```csharp
var builder = client
    .UsingBase()
    .WithQueryParameter("id", new object?[] { 10, 20, "hello" });
// produces: ?id=10&id=20&id=hello
```

* Converts each value using `ToString()`.
* Supports mixed data types.
* Each value is serialized as its own parameter.


## Multiple Parameters

These overloads allow bulk additions for scenarios like configuration-driven queries.

### Key/value pairs (string values)

```csharp
var parameters = new[]
{
    new KeyValuePair<string, string?>("sort", "asc"),
    new KeyValuePair<string, string?>("filter", "active")
};

var builder = client
    .UsingBase()
    .WithQueryParameters(parameters);
```

* Adds each key/value pair.
* Values can be `null`.

### Key/value pairs (object values)

```csharp
var parameters = new[]
{
    new KeyValuePair<string, object?>("page", 2),
    new KeyValuePair<string, object?>("pageSize", 50)
};

var builder = client
    .UsingBase()
    .WithQueryParameters(parameters);
```

* Converts each value using `ToString()`.

### Keys mapped to multiple string values

```csharp
var map = new[]
{
    new KeyValuePair<string, IEnumerable<string?>>("tag", new[] { "science", "fiction" })
};

var builder = client
    .UsingBase()
    .WithQueryParameters(map);
```

* Appends each sequence of values for the given key.

### Keys mapped to multiple object values

```csharp
var map = new[]
{
    new KeyValuePair<string, IEnumerable<object?>>("id", new object?[] { 1, 2, 3 })
};
```

* Converts each value using `ToString()`.
* Throws if the value sequence is `null`.

---

## Conditional Parameters

While query parameters can be added using the fluent [`When`](./conditional-configuration.md) syntax, the null-check patterns is common enough that a set of dedicated overloads are provided for it.

### String Input

```csharp
var builder = client
    .UsingBase()
    .WithQueryParameterIfNotNull("q", searchTerm);
```

* Adds the parameter only if `value` is non-null.

### Object values

```csharp
builder.WithQueryParameterIfNotNull("offset", optionalValue);
```

* Converts the value using `ToString()` if non-null.

## Behavior Notes

* Query parameters accumulate on `HttpRequestBuilder.QueryParameters`.
* Multiple values for the same key result in repeated key/value pairs.
* All configuration is deferred until the builder creates the final request.
* Object values always convert through `ToString()`, so formatting rules should be explicit if stability is required.
* Null query parameter values are serialized as flags `?key`.
* Empty query parameter values are serialized as `?key=`

---

## Quick Reference

| Method                                                                          | Purpose                                |
| ------------------------------------------------------------------------------- | -------------------------------------- |
| `WithQueryParameter(string, string?)`                                           | Add a single string value.             |
| `WithQueryParameter(string, object?)`                                           | Add a single object value (converted). |
| `WithQueryParameter(string, IEnumerable<string?>)`                              | Add multiple string values.            |
| `WithQueryParameter(string, IEnumerable<object?>)`                              | Add multiple object values.            |
| `WithQueryParameters(IEnumerable<KeyValuePair<string, string?>>)`               | Add multiple parameters.               |
| `WithQueryParameters(IEnumerable<KeyValuePair<string, object?>>)`               | Add multiple parameters (converted).   |
| `WithQueryParameters(IEnumerable<KeyValuePair<string, IEnumerable<string?>>> )` | Add sequences of string values.        |
| `WithQueryParameters(IEnumerable<KeyValuePair<string, IEnumerable<object?>>> )` | Add sequences of object values.        |
| `WithQueryParameterIfNotNull(string, string?)`                                  | Conditional single value.              |
| `WithQueryParameterIfNotNull(string, object?)`                                  | Conditional single object value.       |
