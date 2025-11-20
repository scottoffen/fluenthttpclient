namespace FluentHttpClient.Tests;

public class FluentQueryParameterExtensionsTests
{
    private static HttpRequestBuilder CreateBuilder()
    {
        var client = new HttpClient();
        return new HttpRequestBuilder(client, "https://example.com/");
    }

    [Fact]
    public void WithQueryParameter_AddsSingleStringValue_WhenValueProvided()
    {
        var builder = CreateBuilder();

        builder.WithQueryParameter("foo", "bar");

        builder.QueryParameters.Count.ShouldBe(1);
        builder.QueryParameters["foo"].ShouldHaveSingleItem().ShouldBe("bar");
    }

    [Fact]
    public void WithQueryParameter_AddsFlag_WhenStringValueIsNull()
    {
        var builder = CreateBuilder();

        builder.WithQueryParameter("flag", (string?)null);

        builder.QueryParameters.Count.ShouldBe(1);
        var values = builder.QueryParameters["flag"];
        values.ShouldHaveSingleItem().ShouldBe(null);

        var query = builder.QueryParameters.ToQueryString();
        query.ShouldBe("?flag");
    }

    [Fact]
    public void WithQueryParameter_ConvertsObjectValueUsingToString_WhenObjectProvided()
    {
        var builder = CreateBuilder();

        builder.WithQueryParameter("id", 123);

        var values = builder.QueryParameters["id"];
        values.ShouldHaveSingleItem().ShouldBe("123");
    }

    [Fact]
    public void WithQueryParameter_AddsMultipleStringValues_ForSingleKey()
    {
        var builder = CreateBuilder();
        var values = new string?[] { "one", null, "three" };

        builder.WithQueryParameter("tag", values);

        var stored = builder.QueryParameters["tag"];
        stored.Count.ShouldBe(3);
        stored[0].ShouldBe("one");
        stored[1].ShouldBe(null);
        stored[2].ShouldBe("three");

        var query = builder.QueryParameters.ToQueryString();
        query.ShouldBe("?tag=one&tag&tag=three");
    }

    [Fact]
    public void WithQueryParameter_AddsMultipleObjectValues_ForSingleKey()
    {
        var builder = CreateBuilder();
        var values = new object?[] { 1, null, DayOfWeek.Monday };

        builder.WithQueryParameter("p", values);

        var stored = builder.QueryParameters["p"];
        stored.Count.ShouldBe(3);
        stored[0].ShouldBe("1");
        stored[1].ShouldBe(null);
        stored[2].ShouldBe("Monday");
    }

    [Fact]
    public void WithQueryParameters_AddsMultipleKeysSingleValues_WhenStringPairsProvided()
    {
        var builder = CreateBuilder();
        var parameters = new[]
        {
            new KeyValuePair<string, string?>("a", "1"),
            new KeyValuePair<string, string?>("b", null),
        };

        builder.WithQueryParameters(parameters);

        builder.QueryParameters.Count.ShouldBe(2);
        builder.QueryParameters["a"].ShouldHaveSingleItem().ShouldBe("1");
        builder.QueryParameters["b"].ShouldHaveSingleItem().ShouldBe(null);
    }

    [Fact]
    public void WithQueryParameters_AddsMultipleKeysSingleValues_WhenObjectPairsProvided()
    {
        var builder = CreateBuilder();
        var parameters = new[]
        {
            new KeyValuePair<string, object?>("id", 42),
            new KeyValuePair<string, object?>("mode", DayOfWeek.Tuesday),
            new KeyValuePair<string, object?>("flag", null),
        };

        builder.WithQueryParameters(parameters);

        builder.QueryParameters.Count.ShouldBe(3);
        builder.QueryParameters["id"].ShouldHaveSingleItem().ShouldBe("42");
        builder.QueryParameters["mode"].ShouldHaveSingleItem().ShouldBe("Tuesday");
        builder.QueryParameters["flag"].ShouldHaveSingleItem().ShouldBe(null);
    }

    [Fact]
    public void WithQueryParameters_AddsMultipleKeysMultipleValues_WhenStringEnumerablePairsProvided()
    {
        var builder = CreateBuilder();
        var parameters = new[]
        {
            new KeyValuePair<string, IEnumerable<string?>>(
                "tag",
                new string?[] { "one", "two" }),
            new KeyValuePair<string, IEnumerable<string?>>(
                "flag",
                new string?[] { null }),
        };

        builder.WithQueryParameters(parameters);

        builder.QueryParameters.Count.ShouldBe(2);
        var tagValues = builder.QueryParameters["tag"];
        tagValues.Count.ShouldBe(2);
        tagValues[0].ShouldBe("one");
        tagValues[1].ShouldBe("two");

        var flagValues = builder.QueryParameters["flag"];
        flagValues.ShouldHaveSingleItem().ShouldBe(null);

        var query = builder.QueryParameters.ToQueryString();
        query.ShouldBe("?tag=one&tag=two&flag");
    }

    [Fact]
    public void WithQueryParameters_AddsMultipleKeysMultipleValues_WhenObjectEnumerablePairsProvided()
    {
        var builder = CreateBuilder();
        var parameters = new[]
        {
            new KeyValuePair<string, IEnumerable<object?>>(
                "ids",
                new object?[] { 1, 2, 3 }),
            new KeyValuePair<string, IEnumerable<object?>>(
                "mixed",
                new object?[] { null, DayOfWeek.Friday }),
        };

        builder.WithQueryParameters(parameters);

        builder.QueryParameters.Count.ShouldBe(2);

        var idValues = builder.QueryParameters["ids"];
        idValues.Count.ShouldBe(3);
        idValues[0].ShouldBe("1");
        idValues[1].ShouldBe("2");
        idValues[2].ShouldBe("3");

        var mixedValues = builder.QueryParameters["mixed"];
        mixedValues.Count.ShouldBe(2);
        mixedValues[0].ShouldBe(null);
        mixedValues[1].ShouldBe("Friday");
    }

    [Fact]
    public void WithQueryParameters_ThrowsArgumentNullException_WhenNestedValueSequenceIsNull()
    {
        var builder = CreateBuilder();
        var parameters = new[]
        {
            new KeyValuePair<string, IEnumerable<object?>>(
                "ids",
                new object?[] { 1, 2 }),
            new KeyValuePair<string, IEnumerable<object?>>(
                "broken",
                null!),
        };

        Should.Throw<ArgumentNullException>(
            () => builder.WithQueryParameters(parameters));
    }

    [Fact]
    public void WithQueryParameterIfNotNull_AddsValue_WhenStringIsNotNull()
    {
        var builder = CreateBuilder();

        builder.WithQueryParameterIfNotNull("name", "value");

        builder.QueryParameters.Count.ShouldBe(1);
        builder.QueryParameters["name"].ShouldHaveSingleItem().ShouldBe("value");
    }

    [Fact]
    public void WithQueryParameterIfNotNull_DoesNotAddValue_WhenStringIsNull()
    {
        var builder = CreateBuilder();

        builder.WithQueryParameterIfNotNull("name", (string?)null);

        builder.QueryParameters.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void WithQueryParameterIfNotNull_AddsValue_WhenObjectIsNotNull()
    {
        var builder = CreateBuilder();

        builder.WithQueryParameterIfNotNull("count", 5);

        builder.QueryParameters.Count.ShouldBe(1);
        builder.QueryParameters["count"].ShouldHaveSingleItem().ShouldBe("5");
    }

    [Fact]
    public void WithQueryParameterIfNotNull_DoesNotAddValue_WhenObjectIsNull()
    {
        var builder = CreateBuilder();

        builder.WithQueryParameterIfNotNull("count", (object?)null);

        builder.QueryParameters.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void WithQueryParameter_ThrowsArgumentNullException_WhenKeyIsNullForStringOverload()
    {
        var builder = CreateBuilder();
        string key = null!;

        Should.Throw<ArgumentNullException>(
            () => builder.WithQueryParameter(key, "value"));
    }

    [Fact]
    public void WithQueryParameter_ThrowsArgumentNullException_WhenKeyIsNullForObjectOverload()
    {
        var builder = CreateBuilder();
        string key = null!;

        Should.Throw<ArgumentNullException>(
            () => builder.WithQueryParameter(key, new object()));
    }

    [Fact]
    public void WithQueryParameter_ThrowsArgumentNullException_WhenValuesSequenceIsNullForStringEnumerableOverload()
    {
        var builder = CreateBuilder();
        IEnumerable<string?> values = null!;

        Should.Throw<ArgumentNullException>(
            () => builder.WithQueryParameter("key", values));
    }

    [Fact]
    public void WithQueryParameter_ThrowsArgumentNullException_WhenValuesSequenceIsNullForObjectEnumerableOverload()
    {
        var builder = CreateBuilder();
        IEnumerable<object?> values = null!;

        Should.Throw<ArgumentNullException>(
            () => builder.WithQueryParameter("key", values));
    }

    [Fact]
    public void WithQueryParameters_ThrowsArgumentNullException_WhenStringPairSequenceIsNull()
    {
        var builder = CreateBuilder();
        IEnumerable<KeyValuePair<string, string?>> parameters = null!;

        Should.Throw<ArgumentNullException>(
            () => builder.WithQueryParameters(parameters));
    }

    [Fact]
    public void WithQueryParameters_ThrowsArgumentNullException_WhenObjectPairSequenceIsNull()
    {
        var builder = CreateBuilder();
        IEnumerable<KeyValuePair<string, object?>> parameters = null!;

        Should.Throw<ArgumentNullException>(
            () => builder.WithQueryParameters(parameters));
    }

    [Fact]
    public void WithQueryParameters_ThrowsArgumentNullException_WhenStringEnumerablePairSequenceIsNull()
    {
        var builder = CreateBuilder();
        IEnumerable<KeyValuePair<string, IEnumerable<string?>>> parameters = null!;

        Should.Throw<ArgumentNullException>(
            () => builder.WithQueryParameters(parameters));
    }

    [Fact]
    public void WithQueryParameters_ThrowsArgumentNullException_WhenObjectEnumerablePairSequenceIsNull()
    {
        var builder = CreateBuilder();
        IEnumerable<KeyValuePair<string, IEnumerable<object?>>> parameters = null!;

        Should.Throw<ArgumentNullException>(
            () => builder.WithQueryParameters(parameters));
    }
}
