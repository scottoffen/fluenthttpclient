---
sidebar_position: 9
title: Conditional Configuration
---

FluentHttpClient provides conditional configuration extensions that allow you to apply parts of a request pipeline only when certain conditions are met. These methods keep branching logic *inside* the fluent chain rather than scattering `if` statements before or between calls.

Conditional configuration is especially useful for:

* **Feature flags** – enabling or disabling request behavior based on toggles.
* **Environment- or tenant-specific configuration** – applying headers, cookies, or query parameters only for certain tenants or environments.
* **Optional request metadata** – adding correlation IDs, pagination tokens, or custom options only when values are present.
* **Late-bound conditions** – evaluating runtime state at the moment the `HttpRequestMessage` is built, instead of when the fluent pipeline is constructed.
* **Reusable request builders** – where the same configured `HttpRequestBuilder` instance might be used for many requests under different conditions.

FluentHttpClient offers two flavors of conditional configuration: **eager** (`bool`) and **deferred** (`Func<bool>`).

## Eager Condition (`bool`)

Eager conditions are evaluated immediately, at the time the fluent method is called.

Use this when the condition is already known and will *not* change by the time the request is sent.

### Example

```csharp
builder
    .When(isAuthenticated, b => b.WithHeader("X-User", userName))
    .When(debugEnabled, b => b.WithQueryParameter("debug", "true"));
```

Characteristics:

* The condition is evaluated immediately.
* The configuration action executes immediately if true.
* No deferred behavior.
* Equivalent to a normal `if` block, but maintains fluent readability.

Good for:

* Flags determined at build time.
* Optional parameters that are already known.
* Simple, always-synchronous decisions.

## Deferred Condition (`Func<bool>`)

Deferred conditions are evaluated **later**, when the `HttpRequestMessage` is built.

This is ideal for logic that depends on late-bound state that isn’t available when constructing the fluent pipeline.

### Example

```csharp
builder.When(
    predicate: () => FeatureFlags.EnableNewRouting,
    configure: b => b.WithHeader("X-Routing", "new")
);
```

Or:

```csharp
builder.When(
    () => AmbientContext.TenantId is not null,
    b => b.WithQueryParameter("tenant", AmbientContext.TenantId)
);
```

Characteristics:

* Predicate runs when the request is built, not when the fluent call is made.
* The configuration action is stored in `builder.DeferredConfigurators`.
* Useful when a single `HttpRequestBuilder` is reused multiple times with different runtime conditions.

Good for:

* Ambient or contextual values (tenant, correlation ID, locale).
* A/B testing or runtime feature flags.
* Conditionals that depend on runtime data, DI-scoped services, or thread-local context.
* Pipelines where the request isn’t built until significantly later.

## Help Selecting Conditionals

| Scenario                                                 | Use eager (`bool`) | Use deferred (`Func<bool>`) |
| -------------------------------------------------------- | ------------------ | --------------------------- |
| Condition known at pipeline creation time                | ✔️                 |                             |
| Condition depends on runtime values                      |                    | ✔️                          |
| Condition may change between requests using same builder |                    | ✔️                          |
| Simple inline optional behavior                          | ✔️                 | ✔️ (if late-bound)          |
| Condition depends on ambient context                     |                    | ✔️                          |

## Behavior Notes

* Deferred configurators execute in the order they were added.
* Deferred conditions run per-request, making them safe for reusable builders.
* Eager conditions apply configuration immediately and do not affect future requests.
* Both versions preserve fluent readability and help avoid branching outside the fluent chain.

---

## Quick Reference

| Method                                         | Purpose                                                                       |
| ---------------------------------------------- | ----------------------------------------------------------------------------- |
| `When(bool, Action<HttpRequestBuilder>)`       | Apply configuration immediately when the condition is true.                   |
| `When(Func<bool>, Action<HttpRequestBuilder>)` | Apply configuration when the request is built and the predicate returns true. |
