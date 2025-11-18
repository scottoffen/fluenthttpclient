---
sidebar_position: 7
title: Configure Options
---

FluentHttpClient exposes methods for configuring `HttpRequestMessage.Options` through `HttpRequestBuilder`. `HttpRequestOptions` provide a type-safe, per-request bag for custom values that can be consumed by handlers, middleware, or downstream components.

Typical uses include:

* Passing **per-request flags** or feature switches to custom `HttpMessageHandler` implementations.
* Providing **correlation IDs** or **tenant identifiers** that are not intended to be sent over the wire as headers.
* Attaching **context objects** (e.g., retry hints, feature toggles, diagnostics state) that other parts of the pipeline can read.

:::important

These extensions are available on NET 5.0 and later.

:::

## Adding Option Configurators

Use `ConfigureOptions` when you want full control over how `HttpRequestMessage.Options` is modified.

```csharp
builder.ConfigureOptions(options =>
{
    options.Set(new HttpRequestOptionsKey<string>("CorrelationId"), correlationId);
});
```

* Adds an action to `builder.OptionConfigurators`.
* The action is invoked when the final `HttpRequestMessage` is built.
* Throws if `action` is `null`.

This is the most flexible way to work with options, and is a good fit when:

* You need to set multiple options at once.
* You need to conditionally add/remove options based on current state.

## Setting Typed Option Values

For the common case of setting a single typed value, use `WithOption`.

```csharp
var key = new HttpRequestOptionsKey<string>("CorrelationId");

builder.WithOption(key, correlationId);
```

* Adds a configurator that sets the specified key/value pair on `HttpRequestMessage.Options`.
* Uses the standard `HttpRequestOptions.Set` behavior:
  * Adding a new key if it does not exist.
  * Overwriting the existing value for the same key.

This method keeps call sites concise while still benefiting from type safety.

## Behavior Notes

* Option configurators are executed when the `HttpRequestMessage` is created, not when the fluent calls are made.
* Multiple configurators are applied in the order they were added.
* Later configurators can overwrite values set by earlier ones.
* Options are **not** automatically sent over the network; they are used for in-process coordination between components that share access to the `HttpRequestMessage`.

---

## Quick Reference

| Method                                         | Purpose                                                     |
| ---------------------------------------------- | ----------------------------------------------------------- |
| `ConfigureOptions(Action<HttpRequestOptions>)` | Add a custom configurator for `HttpRequestMessage.Options`. |
| `WithOption<T>(HttpRequestOptionsKey<T>, T)`   | Set a single typed option value.                            |
