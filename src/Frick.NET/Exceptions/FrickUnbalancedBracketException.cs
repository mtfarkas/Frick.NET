namespace Frick.NET.Exceptions
{
    /// <summary>
    /// Exception thrown when the interpreter encounters an unbalanced (i.e. not closed) bracket.
    /// </summary>
    /// <param name="bracketPos"></param>
    [Serializable]
    public class FrickUnbalancedBracketException(int bracketPos)
        : Exception($"Unexpected EOF; Unbalanced loop instruction found at position {bracketPos}")
    {
    }
}
