using Frick.NET.Exceptions;

namespace Frick.NET.Internals
{
    /// <summary>
    /// Represents the internal state of the brainfuck interpreter. Internal use only.
    /// </summary>
    internal class FrickInterperterState(int cellSize,
                                         CellOverflowBehaviour cellOverflowBehaviour,
                                         CellValueOverflowBehaviour cellValueOverflowBehaviour)
    {
        // what the interpreter does when it encounters a cell pointer overflow
        private readonly CellOverflowBehaviour _cellOverflowBehaviour = cellOverflowBehaviour;
        // what the interpreter does when it encounters a cell value overflow
        private readonly CellValueOverflowBehaviour _cellValueOverflowBehaviour = cellValueOverflowBehaviour;
        // The "memory" model of the brainfuck interpreter
        public byte[] Cells { get; } = new byte[cellSize];
        // Which cell is currently being pointed to
        public int CurrentPointer { get; private set; } = 0;

        /// <summary>
        /// Move the currently pointed to cell one to the left.
        /// </summary>
        public void MovePointerLeft() =>
            MovePointer(CurrentPointer - 1);

        /// <summary>
        /// Move the currently pointed to cell one to the right.
        /// </summary>
        public void MovePointerRight() => 
            MovePointer(CurrentPointer + 1);

        /// <summary>
        /// Increment the currently pointed to cell's value by 1.
        /// </summary>
        public void IncrementCell() =>
            ChangeCellValue(GetCurrentValue() + 1);

        /// <summary>
        /// Decrement the currently pointed to cell's value by 1.
        /// </summary>
        public void DecrementCell() =>
            ChangeCellValue(GetCurrentValue() - 1);

        /// <summary>
        /// Set the currently pointed to cell's value to an arbitrary value. Generally should be constrained to a byte's size
        /// but the actual behaviour depends on the <see cref="CellValueOverflowBehaviour"/> set on the interpreter.
        /// </summary>
        public void SetCell(int b) =>
            ChangeCellValue(b);

        /// <summary>
        /// Returns the currently pointed to cell's value.
        /// </summary>
        /// <returns></returns>
        public byte GetCurrentValue() =>
            Cells[CurrentPointer];

        /// <summary>
        /// Moves the pointer to an arbitrary new place. The actual behaviour depends on the <see cref="CellOverflowBehaviour"/> set on the interpreter.
        /// </summary>
        private void MovePointer(int newValue)
        {
            if (newValue < 0)
            {
                switch (_cellOverflowBehaviour)
                {
                    case CellOverflowBehaviour.Ignore:
                        return;
                    case CellOverflowBehaviour.WrapAround:
                        newValue = Cells.Length - 1;
                        break;
                    case CellOverflowBehaviour.ThrowException:
                        throw new FrickCellOverflowException(newValue);
                }
            }
            else if (newValue >= Cells.Length)
            {
                switch (_cellOverflowBehaviour)
                {
                    case CellOverflowBehaviour.Ignore:
                        return;
                    case CellOverflowBehaviour.WrapAround:
                        newValue = 0;
                        break;
                    case CellOverflowBehaviour.ThrowException:
                        throw new FrickCellOverflowException(newValue);
                }
            }

            CurrentPointer = newValue;
        }

        /// <summary>
        /// Set the currently pointed to cell's value to an arbitrary value. The actual behaviour depends on the <see cref="CellOverflowBehaviour"/> set on the interpreter.
        /// </summary>
        private void ChangeCellValue(int newValue)
        {
            if (newValue > byte.MaxValue)
            {
                switch (_cellValueOverflowBehaviour)
                {
                    case CellValueOverflowBehaviour.Ignore:
                        return;
                    case CellValueOverflowBehaviour.WrapAround:
                        newValue = byte.MinValue;
                        break;
                    case CellValueOverflowBehaviour.ThrowException:
                        throw new FrickCellValueOverflowException(newValue);
                }
            }
            else if (newValue < byte.MinValue)
            {
                switch (_cellValueOverflowBehaviour)
                {
                    case CellValueOverflowBehaviour.Ignore:
                        return;
                    case CellValueOverflowBehaviour.WrapAround:
                        newValue = byte.MaxValue;
                        break;
                    case CellValueOverflowBehaviour.ThrowException:
                        throw new FrickCellValueOverflowException(newValue);
                }
            }
            Cells[CurrentPointer] = Convert.ToByte(newValue);
        }

        /// <summary>
        /// Resets the state of the interpreter. Sets all cell values to 0 and moves the pointer to 0.
        /// </summary>
        public void Reset()
        {
            Array.Clear(Cells);
            CurrentPointer = 0;
        }
    }
}
