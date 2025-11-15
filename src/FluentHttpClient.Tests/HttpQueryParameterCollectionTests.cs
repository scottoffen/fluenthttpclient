namespace FluentHttpClient.Tests;

public class HttpQueryParameterCollectionTests
{
    [Fact]
    public void IsEmpty_ReturnsTrue_WhenCollectionIsNew()
    {
        var collection = new HttpQueryParameterCollection();

        collection.IsEmpty.ShouldBeTrue();
        collection.Count.ShouldBe(0);
    }

    [Fact]
    public void Add_AddsSingleValue_WhenKeyIsValid()
    {
        var collection = new HttpQueryParameterCollection();

        collection.Add("key", "value");

        collection.IsEmpty.ShouldBeFalse();
        collection.Count.ShouldBe(1);
        collection["key"].ShouldBe(new[] { "value" });
    }

    [Fact]
    public void Add_AddsMultipleValues_WhenCalledMultipleTimesForSameKey()
    {
        var collection = new HttpQueryParameterCollection();

        collection.Add("key", "value1");
        collection.Add("key", "value2");

        collection.Count.ShouldBe(1);
        collection["key"].ShouldBe(new[] { "value1", "value2" });
    }

    [Fact]
    public void Add_AddsMultipleValues_WhenEnumerableOverloadUsed()
    {
        var collection = new HttpQueryParameterCollection();

        collection.AddRange("key", new[] { "value1", "value2" });

        collection.Count.ShouldBe(1);
        collection["key"].ShouldBe(new[] { "value1", "value2" });
    }

    [Fact]
    public void Set_ReplacesExistingValues_WhenKeyAlreadyExists()
    {
        var collection = new HttpQueryParameterCollection();

        collection.Add("key", "value1");
        collection.Set("key", "value2");

        collection.Count.ShouldBe(1);
        collection["key"].ShouldBe(new[] { "value2" });
    }

    [Fact]
    public void Set_ReplacesExistingValues_WhenEnumerableOverloadUsed()
    {
        var collection = new HttpQueryParameterCollection();

        collection.Add("key", "value1");
        collection.SetRange("key", new[] { "value2", "value3" });

        collection.Count.ShouldBe(1);
        collection["key"].ShouldBe(new[] { "value2", "value3" });
    }

    [Fact]
    public void Remove_RemovesKeyAndReturnsTrue_WhenKeyExists()
    {
        var collection = new HttpQueryParameterCollection();
        collection.Add("key", "value");

        var result = collection.Remove("key");

        result.ShouldBeTrue();
        collection.ContainsKey("key").ShouldBeFalse();
        collection.Count.ShouldBe(0);
    }

    [Fact]
    public void Remove_ReturnsFalse_WhenKeyDoesNotExist()
    {
        var collection = new HttpQueryParameterCollection();

        var result = collection.Remove("missing");

        result.ShouldBeFalse();
    }

    [Fact]
    public void Clear_RemovesAllParameters_WhenCalled()
    {
        var collection = new HttpQueryParameterCollection();
        collection.Add("key1", "value1");
        collection.Add("key2", "value2");

        collection.Clear();

        collection.Count.ShouldBe(0);
        collection.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void ContainsKey_ReturnsTrue_WhenKeyExists()
    {
        var collection = new HttpQueryParameterCollection();
        collection.Add("key", "value");

        collection.ContainsKey("key").ShouldBeTrue();
    }

    [Fact]
    public void ContainsKey_ReturnsFalse_WhenKeyDoesNotExist()
    {
        var collection = new HttpQueryParameterCollection();

        collection.ContainsKey("key").ShouldBeFalse();
    }

    [Fact]
    public void TryGetValues_ReturnsTrueAndValues_WhenKeyExists()
    {
        var collection = new HttpQueryParameterCollection();
        collection.AddRange("key", new[] { "value1", "value2" });

        var result = collection.TryGetValues("key", out var values);

        result.ShouldBeTrue();
        values.ShouldBe(new[] { "value1", "value2" });
    }

    [Fact]
    public void TryGetValues_ReturnsFalseAndEmptyList_WhenKeyDoesNotExist()
    {
        var collection = new HttpQueryParameterCollection();

        var result = collection.TryGetValues("missing", out var values);

        result.ShouldBeFalse();
        values.ShouldBeEmpty();
    }

    [Fact]
    public void Indexer_ReturnsValues_WhenKeyExists()
    {
        var collection = new HttpQueryParameterCollection();
        collection.Add("key", "value");

        var values = collection["key"];

        values.ShouldBe(new[] { "value" });
    }

    [Fact]
    public void Indexer_ThrowsKeyNotFound_WhenKeyDoesNotExist()
    {
        var collection = new HttpQueryParameterCollection();

        Should.Throw<KeyNotFoundException>(() =>
        {
            var _ = collection["missing"];
        });
    }

    [Fact]
    public void Add_ThrowsArgumentNullException_WhenKeyIsNull()
    {
        var collection = new HttpQueryParameterCollection();

        Should.Throw<ArgumentNullException>(() => collection.Add(null!, "value"));
    }

    [Fact]
    public void Add_ThrowsArgumentException_WhenKeyIsWhitespace()
    {
        var collection = new HttpQueryParameterCollection();

        Should.Throw<ArgumentException>(() => collection.Add("   ", "value"));
    }

    [Fact]
    public void AddEnumerable_ThrowsArgumentNullException_WhenValuesIsNull()
    {
        var collection = new HttpQueryParameterCollection();

        Should.Throw<ArgumentNullException>(() => collection.AddRange("key", (IEnumerable<string?>)null!));
    }

    [Fact]
    public void SetEnumerable_ThrowsArgumentNullException_WhenValuesIsNull()
    {
        var collection = new HttpQueryParameterCollection();

        Should.Throw<ArgumentNullException>(() => collection.SetRange("key", (IEnumerable<string?>)null!));
    }

    [Fact]
    public void Remove_ThrowsArgumentNullException_WhenKeyIsNull()
    {
        var collection = new HttpQueryParameterCollection();

        Should.Throw<ArgumentNullException>(() => collection.Remove(null!));
    }

    [Fact]
    public void ContainsKey_ThrowsArgumentNullException_WhenKeyIsNull()
    {
        var collection = new HttpQueryParameterCollection();

        Should.Throw<ArgumentNullException>(() => collection.ContainsKey(null!));
    }

    [Fact]
    public void TryGetValues_ThrowsArgumentNullException_WhenKeyIsNull()
    {
        var collection = new HttpQueryParameterCollection();

        Should.Throw<ArgumentNullException>(() =>
        {
            collection.TryGetValues(null!, out _);
        });
    }

    [Fact]
    public void ToQueryString_ReturnsEmptyString_WhenCollectionIsEmpty()
    {
        var collection = new HttpQueryParameterCollection();

        var query = collection.ToQueryString();

        query.ShouldBe(string.Empty);
    }

    [Fact]
    public void ToQueryString_RendersSingleKeyValuePair_WhenOneParameter()
    {
        var collection = new HttpQueryParameterCollection();
        collection.Add("key", "value");

        var query = collection.ToQueryString();

        query.ShouldBe("?key=value");
    }

    [Fact]
    public void ToQueryString_RendersMultipleParameters_WhenMultipleKeysAndValues()
    {
        var collection = new HttpQueryParameterCollection();
        collection.Add("a", "1");
        collection.Add("b", "2");

        var query = collection.ToQueryString();

        query.ShouldBe("?a=1&b=2");
    }

    [Fact]
    public void ToQueryString_RendersMultipleValuesForSameKey_WhenMultipleValuesPresent()
    {
        var collection = new HttpQueryParameterCollection();
        collection.Add("key", "value1");
        collection.Add("key", "value2");

        var query = collection.ToQueryString();

        query.ShouldBe("?key=value1&key=value2");
    }

    [Fact]
    public void ToQueryString_EncodesKeysAndValues_WhenTheyContainSpecialCharacters()
    {
        var collection = new HttpQueryParameterCollection();
        collection.Add("sp ce", "a+b&c=d");

        var query = collection.ToQueryString();

        query.ShouldBe("?sp%20ce=a%2Bb%26c%3Dd");
    }

    [Fact]
    public void ToQueryString_RendersFlagParameter_WhenValueIsNull()
    {
        var collection = new HttpQueryParameterCollection();
        collection.Add("flag", null);

        var query = collection.ToQueryString();

        query.ShouldBe("?flag");
    }

    [Fact]
    public void ToQueryString_RendersFlagParameter_WhenNoValuesPresent()
    {
        var collection = new HttpQueryParameterCollection();

        collection.SetRange("empty", Array.Empty<string?>());

        var query = collection.ToQueryString();

        query.ShouldBe("?empty");
    }

    [Fact]
    public void GetEnumerator_EnumeratesAllParameters_WhenCollectionIsNotEmpty()
    {
        var collection = new HttpQueryParameterCollection();
        collection.Add("a", "1");
        collection.Add("b", "2");
        collection.Add("b", "3");

        var entries = collection.ToList();

        entries.Count.ShouldBe(2);
        entries.Single(kvp => kvp.Key == "a").Value.ShouldBe(new[] { "1" });
        entries.Single(kvp => kvp.Key == "b").Value.ShouldBe(new[] { "2", "3" });
    }
}
