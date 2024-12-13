using DiceEngine.Expressions;
using DiceEngine.Tokenization.Tokens;
using System.Collections;
using System.Collections.Immutable;

namespace DiceEngine.Parsing;

public partial class Parser
{
    private readonly Dictionary<CacheKey, IExpression> _cache = [];

    public ImmutableArray<IToken>[] Cache => [.. _cache.Keys.Select(x => x.Value)];

    /// <summary>
    /// Determines whether the cache of the <see cref="Parser"/> contains a specified expression <see cref="string"/>.
    /// </summary>
    /// <param name="expression">The expression to locate in the cache.</param>
    /// <returns>
    /// <see langword="true"/> if the cache contains an entry with the specified expression; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    public bool ContainsCache(string expression)
        => ContainsCache(_tokenizer.Tokenize(expression));

    /// <summary>
    /// Determines whether the cache of the <see cref="Parser"/> contains a specified token sequence.
    /// </summary>
    /// <param name="expression">The expression to locate in the cache.</param>
    /// <returns>
    /// <see langword="true"/> if the cache contains an entry with the specified expression; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    public bool ContainsCache(IEnumerable<IToken> expression)
        => _cache.ContainsKey(expression.ToImmutableArray());

    /// <summary>
    /// Add expression <see cref="string"/> to the cache of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="key">The expression <see cref="string"/> of the cached <see cref="IExpression"/>.</param>
    /// <param name="value">The cached <see cref="IExpression"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IExpression"/> was successfully added to the cache; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    public bool AddCache(string key, IExpression value)
        => AddCache(_tokenizer.Tokenize(key), value);

    /// <summary>
    /// Add a token sequence to the cache of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="key">The token sequence of the cached <see cref="IExpression"/>.</param>
    /// <param name="value">The cached <see cref="IExpression"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IExpression"/> was successfully added to the cache; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    public bool AddCache(IEnumerable<IToken> key, IExpression value)
    {
        try
        {
            _cache[key.ToImmutableArray()] = value;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Removes a cached <see cref="IExpression"/> based on the specified expression <see cref="string"/>.
    /// </summary>
    /// <param name="expression">The expression <see cref="string"/> of the cached <see cref="IExpression"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IExpression"/> was successfully removed from the cache; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool RemoveCache(string expression)
        => RemoveCache(_tokenizer.Tokenize(expression));

    /// <summary>
    /// Removes a cached <see cref="IExpression"/> based on the specified token sequence.
    /// </summary>
    /// <param name="expression">The token sequence of the cached <see cref="IExpression"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IExpression"/> was successfully removed from the cache; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool RemoveCache(IEnumerable<IToken> expression)
    {
        ImmutableArray<IToken> sequence = [.. expression];

        return _cache.ContainsKey(sequence) && _cache.Remove(sequence);
    }

    /// <summary>
    /// Clears the cache of the parser.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the cache was successfully cleared; otherwise <see langword="false"/>.
    /// </returns>
    public bool ClearCache()
    {
        _cache.Clear();
        return _cache.Count == 0;
    }

    private readonly struct CacheKey(ImmutableArray<IToken> value) : IEnumerable<IToken>
    {
        public readonly ImmutableArray<IToken> Value = value;

        public readonly override bool Equals(object? obj)
            => obj is IEnumerable<IToken> enumerable && enumerable.SequenceEqual(Value);

        public readonly override int GetHashCode()
            => Value.Select(x => x.GetHashCode()).Aggregate((a, b) => HashCode.Combine(a, b));

        public readonly IEnumerator<IToken> GetEnumerator() => ((IEnumerable<IToken>)Value).GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Value).GetEnumerator();

        public static implicit operator CacheKey(ImmutableArray<IToken> value)
            => new CacheKey(value);
    }
}
