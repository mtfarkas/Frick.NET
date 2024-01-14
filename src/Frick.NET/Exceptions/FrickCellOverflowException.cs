namespace Frick.NET.Exceptions
{
    /// <summary>
    /// Exception thrown when the cell pointer is moved outside of the bounds of the cell memory, and the interpreter is configured to produce an error.
    /// </summary>
    /// <param name="requestedCell"></param>
    [Serializable]
	public class FrickCellOverflowException(int requestedCell)
        : Exception($"Cell overflow error. The interpreter is configured to throw exceptions on cell overflow. The requested cell value: {requestedCell}")
    {
	}
}
