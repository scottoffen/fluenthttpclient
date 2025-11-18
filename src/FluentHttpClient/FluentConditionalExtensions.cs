namespace FluentHttpClient;

/// <summary>
/// Provides conditional configuration helpers for <see cref="HttpRequestBuilder"/> instances.
/// </summary>
/// <remarks>
/// These extension methods allow callers to apply additional configuration to a request
/// only when a specified condition or predicate evaluates to true. The <c>bool</c>-based
/// overloads apply configuration immediately when the fluent call is made, while the
/// predicate-based overloads defer evaluation until the <see cref="HttpRequestMessage"/>
/// is actually built.
/// </remarks>
public static class FluentConditionalExtensions
{
    /// <summary>
    /// Applies the specified configuration action to the builder when the condition is true.
    /// </summary>
    /// <remarks>
    /// The condition is evaluated immediately and the configuration action is invoked
    /// at the time this method is called. This is equivalent to a simple <c>if</c> check
    /// around the configuration logic, but keeps the control flow within the fluent
    /// pipeline.
    /// </remarks>
    /// <param name="builder"></param>
    /// <param name="condition"></param>
    /// <param name="configure"></param>
    public static HttpRequestBuilder When(
        this HttpRequestBuilder builder,
        bool condition,
        Action<HttpRequestBuilder> configure)
    {
        Guard.AgainstNull(configure, nameof(configure));

        if (condition)
        {
            configure(builder);
        }

        return builder;
    }

    /// <summary>
    /// Applies the specified configuration action to the builder when the predicate returns true.
    /// </summary>
    /// <remarks>
    /// The predicate is evaluated when the <see cref="HttpRequestMessage"/> is built,
    /// not when this method is called. The configuration action is added to the
    /// <see cref="HttpRequestBuilder.DeferredConfigurators"/> collection and will run
    /// for each built request where the predicate evaluates to true. This is useful
    /// for conditions that depend on late-bound state such as ambient context values,
    /// feature flags, or other runtime information only available at request creation time.
    /// </remarks>
    /// <param name="builder"></param>
    /// <param name="predicate"></param>
    /// <param name="configure"></param>
    public static HttpRequestBuilder When(
        this HttpRequestBuilder builder,
        Func<bool> predicate,
        Action<HttpRequestBuilder> configure)
    {
        Guard.AgainstNull(predicate, nameof(predicate));
        Guard.AgainstNull(configure, nameof(configure));

        builder.DeferredConfigurators.Add(b =>
        {
            if (predicate())
            {
                configure(b);
            }
        });

        return builder;
    }
}
