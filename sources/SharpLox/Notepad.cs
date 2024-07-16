/*

public interface IStringEnumeratorFactory
{
    IStringEnumerator CreateStringEnumerator(string @string);
}

public interface IStringEnumerator : IExtendedEnumerator<char>
{
    char? CurrentChar { get; }
    
    char? NextChar { get; }
}

public interface IExtendedEnumerator<out T> : IEnumerator<T>
{
    new T? Current { get; }
    
    T? Next { get; }
    
    int CurrentIndex { get; }
    
    bool IsAtEnd();
}

public static class ExtendedEnumeratorExtensions
{
    public static bool MatchCurrent<T>(this IExtendedEnumerator<T> enumerator, T expected)
    {
        ArgumentNullException.ThrowIfNull(expected, nameof(expected));
        
        return expected.Equals(enumerator.Current);
    }
    
    public static bool MatchCurrentAndMoveNextIfMatchedAndNotIsAtEnd<T>(this IExtendedEnumerator<T> enumerator, T expected)
    {
        ArgumentNullException.ThrowIfNull(expected, nameof(expected));
        
        var isMatched = expected.Equals(enumerator.Current);

        if (isMatched && !enumerator.IsAtEnd())
            enumerator.MoveNext();

        return isMatched;
    }
}

public interface ISimpleStringEnumerator
{
    bool IsAtEnd();
    
    void Advance();

    char Peek();

    char PeekNext();

    char Match(char expectedChar);
}

 */