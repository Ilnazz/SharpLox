using System.Collections.Generic;
using SharpLox.Tokens;

namespace SharpLox.Interpretation;

public sealed class Environment(IEnvironment? enclosing) : IEnvironment
{
    #region Fields
    private readonly IDictionary<string, object?> _valueByNames = new Dictionary<string, object?>();
    
    private readonly object _nullObject = new();
    #endregion

    #region Public methods
    public void Define(string name) =>
        _valueByNames[name] = _nullObject;

    public void Assign(Token name, object? value)
    {
        if (_valueByNames.ContainsKey(name.Lexeme!))
        {
            _valueByNames[name.Lexeme!] = value;
            return;
        }

        if (enclosing is not null)
        {
            enclosing.Assign(name, value);
            return;
        }
        
        throw CreateUndefinedException(name);
    }

    public object? Get(Token name)
    {
        if (_valueByNames.TryGetValue(name.Lexeme!, out var value))
        {
            if (value == _nullObject)
                throw CreateUninitializedException(name);

            return value;
        }
        
        if (enclosing is not null)
            return enclosing.Get(name);
            
        throw CreateUndefinedException(name);
    }
    #endregion

    #region Private methods
    private static RuntimeException CreateUndefinedException(Token name) =>
        new(name, $"Undefined variable \"{name.Lexeme}\".");
    
    private static RuntimeException CreateUninitializedException(Token name) =>
        new(name, $"Variable \"{name.Lexeme}\" has not been initialized.");
    #endregion
}