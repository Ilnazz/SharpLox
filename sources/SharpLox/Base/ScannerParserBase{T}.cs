using System.Collections.Generic;
using System.Linq;
using SharpLox.Errors;

namespace SharpLox.Base;

public abstract class ScannerParserBase<T>(IErrorReporter errorReporter, IReadOnlyList<T> collection)
{
    #region Protected properties
    protected int CurrentIndex { get; set; }

    protected T Previous => Peek(amount: -1);

    protected T Current => Peek();

    protected T Next => Peek(amount: 1);
    #endregion

    #region Peek
    protected T PeekAndAdvance(int amount = 0)
    {
        var item = Peek(amount);
        Advance();
        return item;
    }
    
    // Todo: just return default, create the "defaultValue" parameter or make it nullable?
    // Do not throw exception because it will require tedious manual checks in implementations...
    protected T Peek(int amount = 0)
    {
        var index = CurrentIndex + amount;
        return IsValidIndex(index) ? collection[index] : default!;
    }

    // Todo: what about case when collection.Count is 0?
    private bool IsValidIndex(int index) =>
        index >= 0 && index <= collection.Count - 1;
    #endregion

    #region Match any
    protected bool MatchAnyAndAdvance(params T[] expectedItems)
    {
        if (!MatchAny(expectedItems))
            return false;
        
        Advance();
        return true;
    }
    
    protected bool MatchAny(params T[] expectedItems) => expectedItems.Any(Match);
    #endregion

    #region Match
    protected bool MatchAndAdvance(T expectedItem)
    {
        if (!Match(expectedItem))
            return false;
        
        Advance();
        return true;
    }

    protected bool Match(T expectedItem) => Equals(Current, expectedItem);
    #endregion

    protected abstract void Advance(int amount = 1);

    protected void ReportError(Error error) => errorReporter.ReportError(error);
}