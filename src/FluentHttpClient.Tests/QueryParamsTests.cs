using Shouldly;

namespace FluentHttpClient.Tests;

public class QueryParamsTests
{
    [Fact]
    public void DoesNotRemoveEmptyValues()
    {
        FluentHttpClientOptions.RemoveEmptyQueryParameters = false;

        var expected = "?name=bob&age=&color=blue";

        var qp = new QueryParams
        {
            { "name", "bob" },
            { "age", "" },
            { "color", "blue" }
        };

        var actual = qp.ToString();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void RemovesEmptyValues()
    {
        FluentHttpClientOptions.RemoveEmptyQueryParameters = true;

        var expected = "?name=bob&color=blue";

        var qp = new QueryParams
        {
            { "name", "bob" },
            { "age", "" },
            { "color", "blue" }
        };

        var actual = qp.ToString();
        actual.ShouldBe(expected);
    }
}
