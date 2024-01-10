using Shouldly;

namespace FluentHttpClient.Tests;

public class NullHttpResponseMessageTests
{
    [Fact]
    public void IsSuccessStatusCodeTest()
    {
        var message = new NullHttpResponseMessage();
        message.IsSuccessStatusCode.ShouldBeFalse();
    }

    [Fact]
    public void HttpResponseExceptionOccurredTest()
    {
        HttpResponseMessage message = new NullHttpResponseMessage();
        message.HttpResponseMessageExceptionOccurred().ShouldBeTrue();
    }
}