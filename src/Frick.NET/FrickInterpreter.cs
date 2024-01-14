using Frick.NET.Exceptions;
using Frick.NET.Internals;

namespace Frick.NET
{
    /// <summary>
    /// The brainfuck language interpreter
    /// </summary>
    public class FrickInterpreter
    {
        // what opcodes are recognized
        private static readonly HashSet<char> _validInstructions = [
            Instructions.MOVE_POINTER_LEFT,
            Instructions.MOVE_POINTER_RIGHT,
            Instructions.INCREMENT_CELL,
            Instructions.DECREMENT_CELL,
            Instructions.OUTPUT_CELL,
            Instructions.GET_INPUT,
            Instructions.LOOP_START,
            Instructions.LOOP_END,
        ];

        // the internal state of the brainfuck interpreters
        // contains cells and pointer values
        private readonly FrickInterperterState _state;

        /// <summary>
        /// Constructs a brainfuck language interpreter with the given cell size and overflow behaviours
        /// </summary>
        /// <param name="cellSize">How many cells are available for programs. Defaults to 2^15. Cannot be smaller than 1.</param>
        /// <param name="cellOverflowBehaviour">What should happen when a program tries to overflow the available cells.</param>
        /// <param name="cellValueOverflowBehaviour">What should happen when a program tries to overflow the value of a cell.</param>
        /// <exception cref="ArgumentException"></exception>
        public FrickInterpreter(int cellSize = 1 << 15,
                                CellOverflowBehaviour cellOverflowBehaviour = CellOverflowBehaviour.Ignore,
                                CellValueOverflowBehaviour cellValueOverflowBehaviour = CellValueOverflowBehaviour.WrapAround)
        {
            if (cellSize == 0) { throw new ArgumentException("Cell size cannot be smaller than 1.", nameof(cellSize)); }

            _state = new(cellSize, cellOverflowBehaviour, cellValueOverflowBehaviour);
        }

        /// <summary>
        /// Runs a brainfuck program given in the <paramref name="source"/> parameter.
        /// </summary>
        /// <param name="source">The program to run.</param>
        /// <param name="resetState">Flag to reset the internal state of the interpreter when running the source.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exceptions.FrickCellOverflowException"></exception>
        /// <exception cref="Exceptions.FrickCellValueOverflowException"></exception>
        /// <exception cref="Exceptions.FrickUnbalancedBracketException"></exception>
        public void Run(string source, bool resetState = true)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentNullException(nameof(source), "Source code cannot be null or empty.");
            }

            if (resetState) { _state.Reset(); }

            // this is for handling starting indexes of loops in the source
            Stack<int> loopStack = new();

            for (int idx = 0; idx < source.Length; idx++)
            {
                char c = source[idx];

                // ignore non-valid tokens
                if (!IsValidInstruction(c)) { continue; }

                switch (c)
                {
                    case Instructions.MOVE_POINTER_RIGHT:
                        _state.MovePointerRight();
                        break;
                    case Instructions.MOVE_POINTER_LEFT:
                        _state.MovePointerLeft();
                        break;
                    case Instructions.INCREMENT_CELL:
                        _state.IncrementCell();
                        break;
                    case Instructions.DECREMENT_CELL:
                        _state.DecrementCell();
                        break;
                    case Instructions.OUTPUT_CELL:
                        {
                            byte currentCellValue = _state.GetCurrentValue();
                            // TODO: generalize output
                            Console.Write((char)currentCellValue);
                            break;
                        }
                    case Instructions.GET_INPUT:
                        {
                            // TODO: generalize input
                            int readValue = Console.Read();
                            _state.SetCell(readValue);
                            break;
                        }
                    case Instructions.LOOP_START:
                        {
                            // if the currently pointed to value is not 0, the loop will run, so push
                            // the starting index for it onto the stack
                            if (_state.GetCurrentValue() != 0)
                            {
                                loopStack.Push(idx);
                            }
                            // if the currently pointed to value is 0, the loop is skipped
                            // in this case, we should iterate forwards and find the matching bracket
                            // if no matching bracket is found, throw an error
                            else
                            {
                                int loopEnd = FindMatchingBracket(source, idx);
                                if (loopEnd < 0) { throw new FrickUnbalancedBracketException(idx); }
                                idx = loopEnd;
                            }
                            break;
                        }
                    case Instructions.LOOP_END:
                        {
                            // pop the last pushed index off the stack
                            // if the currently pointed to value is not 0, set the text iteration
                            // back to the beginning of this loop, at which point, the position
                            // will be pushed onto the stack again; this repeats until the value pointed to becomes zero
                            int poppedIdx = loopStack.Pop();
                            if (_state.GetCurrentValue() != 0)
                            {
                                idx = poppedIdx - 1;
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Finds a matching bracket "]" starting from the position specified
        /// </summary>
        private static int FindMatchingBracket(string text, int startPos)
        {
            int endPos = startPos;
            int openBracketCount = 1;
            while (openBracketCount > 0 && endPos < text.Length)
            {
                char c = text[++endPos];
                if (c == Instructions.LOOP_START) { openBracketCount++; }
                else if (c == Instructions.LOOP_END) { openBracketCount--; }
            }
            return openBracketCount == 0
                ? endPos
                : -1;
        }

        /// <summary>
        /// Checks if the specified character is a valid operation in brainfuck
        /// </summary>
        private static bool IsValidInstruction(char c) =>
            _validInstructions.Contains(c);
    }
}
