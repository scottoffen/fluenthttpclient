using System.Collections;
using System.Text;

namespace FluentHttpClient;

/// <summary>
/// Represents a collection of HTTP query string parameters that supports multiple values per key.
/// </summary>
/// <remarks>
/// All query string values for <see cref="HttpRequestBuilder"/> are composed from this collection.
/// Keys are case sensitive. Values are not automatically converted from other types.
/// </remarks>
public sealed class HttpQueryParameterCollection :
    IEnumerable<KeyValuePair<string, IReadOnlyList<string?>>>
{
    private readonly Dictionary<string, List<string?>> _parameters =
        new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the number of distinct parameter keys in the collection.
    /// </summary>
    public int Count => _parameters.Count;

    /// <summary>
    /// Gets a value that indicates whether the collection is empty.
    /// </summary>
    public bool IsEmpty => _parameters.Count == 0;

    /// <summary>
    /// Gets the values associated with the specified key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>
    /// A read-only list of values for the key.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the key does not exist in the collection.
    /// </exception>
    public IReadOnlyList<string?> this[string key] => _parameters[key];

    /// <summary>
    /// Adds a parameter value for the specified key.
    /// </summary>
    /// <param name="key">The parameter name.</param>
    /// <param name="value">
    /// The parameter value. If null, the parameter is rendered as a flag
    /// (for example, <c>?key</c>).
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="key"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="key"/> is empty or consists only of white space.
    /// </exception>
    public void Add(string key, string? value)
    {
        ValidateKey(key);

        if (!_parameters.TryGetValue(key, out var list))
        {
            list = new List<string?>();
            _parameters[key] = list;
        }

        list.Add(value);
    }

    /// <summary>
    /// Adds one or more values for the specified key.
    /// </summary>
    /// <param name="key">The parameter name.</param>
    /// <param name="values">The parameter values.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="key"/> or <paramref name="values"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="key"/> is empty or consists only of white space.
    /// </exception>
    public void AddRange(string key, IEnumerable<string?> values)
    {
        Guard.AgainstNull(values, nameof(values));

        ValidateKey(key);

        if (!_parameters.TryGetValue(key, out var list))
        {
            list = new List<string?>();
            _parameters[key] = list;
        }

        foreach (var value in values)
        {
            list.Add(value);
        }
    }

    /// <summary>
    /// Replaces any existing values for the specified key with a single value.
    /// </summary>
    /// <param name="key">The parameter name.</param>
    /// <param name="value">
    /// The parameter value. If null, the parameter is rendered as a flag
    /// (for example, <c>?key</c>).
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="key"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="key"/> is empty or consists only of white space.
    /// </exception>
    public void Set(string key, string? value)
    {
        ValidateKey(key);

        _parameters[key] = new List<string?> { value };
    }

    /// <summary>
    /// Replaces any existing values for the specified key with the provided values.
    /// </summary>
    /// <param name="key">The parameter name.</param>
    /// <param name="values">The parameter values.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="key"/> or <paramref name="values"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="key"/> is empty or consists only of white space.
    /// </exception>
    public void SetRange(string key, IEnumerable<string?> values)
    {
        Guard.AgainstNull(values, nameof(values));

        ValidateKey(key);

        _parameters[key] = new List<string?>(values);
    }

    /// <summary>
    /// Removes all values for the specified key.
    /// </summary>
    /// <param name="key">The parameter name.</param>
    /// <returns>
    /// true if the key was found and removed; otherwise false.
    /// </returns>
    public bool Remove(string key)
    {
        Guard.AgainstNull(key, nameof(key));

        return _parameters.Remove(key);
    }

    /// <summary>
    /// Removes all parameters from the collection.
    /// </summary>
    public void Clear()
    {
        _parameters.Clear();
    }

    /// <summary>
    /// Determines whether the collection contains the specified key.
    /// </summary>
    /// <param name="key">The parameter name.</param>
    /// <returns>
    /// true if the key exists in the collection; otherwise false.
    /// </returns>
    public bool ContainsKey(string key)
    {
        Guard.AgainstNull(key, nameof(key));

        return _parameters.ContainsKey(key);
    }

    /// <summary>
    /// Attempts to get the values associated with the specified key.
    /// </summary>
    /// <param name="key">The parameter name.</param>
    /// <param name="values">
    /// When this method returns, contains the values associated with the key,
    /// or an empty list if the key is not found.
    /// </param>
    /// <returns>
    /// true if the key exists in the collection; otherwise false.
    /// </returns>
    public bool TryGetValues(string key, out IReadOnlyList<string?> values)
    {
        Guard.AgainstNull(key, nameof(key));

        if (_parameters.TryGetValue(key, out var list))
        {
            values = list;
            return true;
        }

        values = [];
        return false;
    }

    /// <summary>
    /// Returns a query string representation of the collection.
    /// </summary>
    /// <returns>
    /// A query string starting with '?', or an empty string if the collection is empty.
    /// </returns>
    /// <remarks>
    /// Keys and values are encoded using <see cref="Uri.EscapeDataString(string)"/>.
    /// Parameters with null values are rendered as flags (for example, <c>?key</c>).
    /// Keys with no values are omitted.
    /// </remarks>
    public string ToQueryString()
    {
        if (_parameters.Count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        var first = true;

        void AppendPrefix()
        {
            if (first)
            {
                builder.Append('?');
                first = false;
            }
            else
            {
                builder.Append('&');
            }
        }

        foreach (var pair in _parameters)
        {
            var key = pair.Key;
            var values = pair.Value;
            var encodedName = Uri.EscapeDataString(key);

            if (values.Count == 0)
            {
                // No values → render as flag: ?key
                AppendPrefix();
                builder.Append(encodedName);
                continue;
            }

            foreach (var value in values)
            {
                AppendPrefix();
                builder.Append(encodedName);

                if (value is not null)
                {
                    builder.Append('=');
                    builder.Append(Uri.EscapeDataString(value));
                }
            }
        }

        return builder.ToString();
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, IReadOnlyList<string?>>> GetEnumerator()
    {
        foreach (var pair in _parameters)
        {
            yield return new KeyValuePair<string, IReadOnlyList<string?>>(pair.Key, pair.Value);
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static void ValidateKey(string key)
    {
        Guard.AgainstNull(key, nameof(key));

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key cannot be empty or consist only of white space.", nameof(key));
        }
    }
}
