namespace Frick.NET
{
    /// <summary>
    /// Represents how the interpreter should behave when encountering a cell pointer overflow.
    /// </summary>
    public enum CellOverflowBehaviour
    {
        /// <summary>
        /// The instruction causing the overflow should be ignored.
        /// </summary>
        Ignore = 1,
        /// <summary>
        /// The pointer should wrap around in the other direction.
        /// </summary>
        WrapAround,
        /// <summary>
        /// The interpreter should throw an exception.
        /// </summary>
        ThrowException
    }
}
