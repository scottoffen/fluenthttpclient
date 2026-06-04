using System.Reflection;
#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace FluentHttpClient;

internal static class BuilderFactory<
#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    TBuilder>
    where TBuilder : HttpRequestBuilder
{
    private static readonly ConstructorInfo _fromClient;
    private static readonly ConstructorInfo _fromRoute;
    private static readonly ConstructorInfo _fromUri;

    static BuilderFactory()
    {
        _fromClient = ResolveCtor(typeof(HttpClient));
        _fromRoute = ResolveCtor(typeof(HttpClient), typeof(string));
        _fromUri = ResolveCtor(typeof(HttpClient), typeof(Uri));
    }

    public static TBuilder Create(HttpClient client)
        => (TBuilder)_fromClient.Invoke(new object[] { client });

    public static TBuilder Create(HttpClient client, string route)
        => (TBuilder)_fromRoute.Invoke(new object[] { client, route });

    public static TBuilder Create(HttpClient client, Uri uri)
        => (TBuilder)_fromUri.Invoke(new object[] { client, uri });

    private static ConstructorInfo ResolveCtor(params Type[] parameterTypes)
    {
        return typeof(TBuilder).GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            binder: null,
            parameterTypes,
            modifiers: null)
            ?? throw new InvalidOperationException(
                $"{typeof(TBuilder)} must declare a public constructor"
                + $" ({string.Join(", ", Array.ConvertAll(parameterTypes, t => t.Name))})"
                + $" to be used with BuilderFactory<{typeof(TBuilder).Name}>.");
    }
}