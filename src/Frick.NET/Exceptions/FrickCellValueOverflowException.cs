namespace Frick.NET.Exceptions
{

    /// <summary>
    /// Exception thrown when the cell value is set to a value not inside the range of an unsigned 8-bit number, and the interpreter is configured to produce an error.
    /// </summary>
    [Serializable]
    public class FrickCellValueOverflowException(int requestedValue) 
        : Exception($"Cell calue overflow error. The interpreter is configured to throw exceptions on cell value overflow. The requested cell index: {requestedValue}")
    {
    }
}
